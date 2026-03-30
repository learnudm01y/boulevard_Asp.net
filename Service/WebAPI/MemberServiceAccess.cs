using Boulevard.BaseRepository;
using Boulevard.Contexts;
using Boulevard.Helper;
using Boulevard.Models;
using Boulevard.RequestModels;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Razor.Tokenizer;

namespace Boulevard.Service.WebAPI
{
    public class MemberServiceAccess
    {
        public IUnitOfWork uow;
        public MemberServiceAccess()
        {
            uow = new UnitOfWork();
        }

        string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/";

        /// <summary>
        /// Member Registration
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<Member> Registration(MemberRequest member)
        {
            try
            {
                var result = MatchObjectHelper.GenerateMatchedObject(member, new Member());
                result.Password = HashConfig.GetHash(member.Password);
                // result.SecurityKey = RandomStringHelper.RandomString(20);
                result.MemberKey = Guid.NewGuid();
                result.SecurityToken = RandomStringHelper.RandomIntegerString(6).ToUpper();
                result.Status = "Active";
                result.CreateDate = DateTimeHelper.DubaiTime();
                result.PhoneCode = "+971";
                result.CreateBy = 1;
                var mem = await uow.MemberRepository.Add(result);


                if (mem != null && !string.IsNullOrEmpty(member.MemberFireBaseToken))
                {
                    MemberFirebase memberFirebase = new MemberFirebase();

                    memberFirebase.MemberId = mem.MemberId;
                    memberFirebase.FirebaseToken = member.MemberFireBaseToken;
                    memberFirebase.Status = "Active";
                    memberFirebase.LastUpdated = DateTimeHelper.DubaiTime();

                    await uow.MemberFireBaseRepository.Add(memberFirebase);
                }

                //string M_Title = "Welcome to tahtaj";

                //string M_Message = "Welcome to Tahtaj. You have successfully verified your account and now you can start posting your Ads in different categories.";


                //PushNotificationAccess.SendInvoiceMemberNotification(mem.MemberId, M_Title, M_Message);

                var ss = await  GetById(Convert.ToInt32(mem.MemberId));
                return ss;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }

        }

        //public static Member IsMemberExist(int memberId)
        //{
        //    var db = new AlbaqalaDbContexts();
        //    return db.Members.FirstOrDefault(e => e.MemberId == memberId);
        //}


        /// <summary>
        /// Member Login
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        /// 

        public async Task<bool> ActiveMember(string email)
        {
            try
            {
                var members = await uow.MemberRepository.Get().Where(s => s.Email == email).FirstOrDefaultAsync();

                members.Status = "Active";

                await uow.MemberRepository.Edit(members);

                return true;


            }
            catch (Exception ex)
            {

                return false;
            }
        }
        public async Task<Member> Login(MemberLogin member)
        {
            try
            {
                var result = new Member();
                var password = HashConfig.GetHash(member.Password);
                 result = await uow.MemberRepository.Get().Where(t => t.Email == member.UserName && t.Password == password && t.Status == "Active").FirstOrDefaultAsync();
                if (result == null)
                {
                    result = await uow.MemberRepository.Get().Where(t => t.PhoneNumber == member.UserName && t.Password == password && t.Status == "Active").FirstOrDefaultAsync();
                }
                if (result != null)
                {
                    result.ThirdPartyLogin = false;
                    await uow.MemberRepository.Edit(result);
                }
                if (result!=null && !string.IsNullOrEmpty(member.FirebaseToken))
                {
                    await UpdateFireBase(Convert.ToInt32(result.MemberId), member.FirebaseToken);
                }

                var ss = new Member();
                if (result != null)
                {
                    ss = await GetById(Convert.ToInt32(result.MemberId));
                }
                //if (ss == null)
                //{ 
                //    ss = await GetById(result.MemberId);
                //}
                return result != null ? ss : new Member();
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task<Member> LoginForThirdPartyAsync(int memberId, string token, string deviceId, string loginFrom)
        {
            try
            {
                var member = await uow.MemberRepository.Get()
                    .FirstOrDefaultAsync(e => e.MemberId == memberId && e.Status == "Active");

                if (member == null)
                    return null;

                member.ThirdPartyLogin = true;
                member.ThirdPartyLoginFrom = loginFrom;
                member.ThirdPartyLoginKey = deviceId;
                member.UpdateDate = DateTime.UtcNow;

                await uow.MemberRepository.Edit(member);


                if (!string.IsNullOrEmpty(token))
                {
                    await UpdateFireBase(Convert.ToInt32(member.MemberId), token);
                }
                //await InsertFirebaseTokenAsync(member.MemberId, token);

                return member;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public async Task<Member> RegisterThirdPartyAsync(MemberRequest member)
        {
            try
            {
                var result = MatchObjectHelper.GenerateMatchedObject(member, new Member());
                //result.Password = HashConfig.GetHash(member.Password);
                // result.SecurityKey = RandomStringHelper.RandomString(20);
                result.MemberKey = Guid.NewGuid();
                result.SecurityToken = RandomStringHelper.RandomIntegerString(6).ToUpper();
                result.Status = "Active";
                result.CreateDate = DateTimeHelper.DubaiTime();
                result.PhoneCode = "+971";
                result.CreateBy = 1;
                result.Image = member.Image;
                result.ThirdPartyLogin = member.ThirdPartyLogin;
                    result.ThirdPartyLoginFrom = member.ThirdPartyLoginFrom;
                    result.ThirdPartyLoginKey = member.ThirdPartyLoginKey;
                var mem = await uow.MemberRepository.Add(result);


                if (mem != null && !string.IsNullOrEmpty(member.MemberFireBaseToken))
                {
                    MemberFirebase memberFirebase = new MemberFirebase();

                    memberFirebase.MemberId = Convert.ToInt32(mem.MemberId);
                    memberFirebase.FirebaseToken = member.MemberFireBaseToken;
                    memberFirebase.Status = "Active";
                    memberFirebase.LastUpdated = DateTimeHelper.DubaiTime();

                    await uow.MemberFireBaseRepository.Add(memberFirebase);
                }

                //string M_Title = "Welcome to tahtaj";

                //string M_Message = "Welcome to Tahtaj. You have successfully verified your account and now you can start posting your Ads in different categories.";


                //PushNotificationAccess.SendInvoiceMemberNotification(mem.MemberId, M_Title, M_Message);

                var ss = await GetById(Convert.ToInt32(mem.MemberId));
                return ss;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }

        }
        //public async Task<Member?> RegisterThirdPartyAsync(MemberRegisterRequest model)
        //{
        //    try
        //    {
        //        var member = new Member
        //        {
        //            MemberKey = Guid.NewGuid(),
        //            Name = model.Name,
        //            MemberType = "Registered",
        //            Email = model.Email,
        //            Password = string.IsNullOrEmpty(model.Password)
        //                ? HashConfig.GetHash("Gentoo@123")
        //                : HashConfig.GetHash(model.Password),
        //            PhoneNumber = model.PhoneNumber,
        //            CountryCode = "+971",
        //            Gender = "Unknown",
        //            Status = "Active",
        //            CreatedAt = DateTime.UtcNow,
        //            UpdatedAt = DateTime.UtcNow,
        //            DateOfBirth = DateTime.Parse("1971-01-01"),
        //            SecurityKey = RandomStringHelper.RandomString(20),
        //            SecurityToken = RandomStringHelper.RandomIntegerString(6),
        //            ThirdPartyLogin = model.ThirdPartyLogin,
        //            ThirdPartyLoginFrom = model.ThirdPartyLoginFrom,
        //            ThirdPartyLoginKey = model.ThirdPartyLoginKey
        //        };

        //        await uow.MemberRepository.Add(member);


        //        if (!string.IsNullOrEmpty(model.FirebaseToken))
        //        {
        //            await UpdateFireBase(member.MemberId, model.FirebaseToken);
        //            await _notificationDataAccess.SendMemberNotificationAsync(member.MemberId,
        //                "Your account has been successfully created. Get ready for hassle-free deliveries!",
        //                "Welcome to Flinto!");
        //        }

        //        //EmailService.SendMemberRegistrationResponse(member);
        //        return member;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

        public async Task ThirpartyLoginFalseAsync(int memberId)
        {
            try
            {
                var member = await uow.MemberRepository.Get().Where(s => s.MemberId == memberId).FirstOrDefaultAsync();
                if (member == null) return;

                member.ThirdPartyLogin = false;
                await uow.MemberRepository.Edit(member);

            }
            catch { }
        }

        public async Task<Member> LoginForOTP(MemberLogin member)
        {
            try
            {
                var result = new Member();
              
                result = await uow.MemberRepository.Get().Where(t => t.PhoneNumber == member.UserName && t.Status == "Active").FirstOrDefaultAsync();
              
                if (!string.IsNullOrEmpty(member.FirebaseToken))
                {
                    await UpdateFireBase(Convert.ToInt32(result.MemberId), member.FirebaseToken);
                }

                var ss = new Member();
                if (result != null)
                {
                    var OTP = RandomStringHelper.RandomInteger(4);

                    result.OTPNumber = OTP.ToString();
                    result.OTPGenerateDateTime = Helper.DateTimeHelper.DubaiTime();


                    await uow.MemberRepository.Edit(result);

                    ss = await GetById(Convert.ToInt32(result.MemberId));
                }
              
                return result != null ? ss : new Member();
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                return null;
            }
        }


        public async Task<Member> OTPCheck(string OTP,long memberId)
        {
            try
            {
                var minDateTime = DateTimeHelper.DubaiTime().AddMinutes(-5);

                var member = await uow.MemberRepository.Get()
                    .Where(t => t.MemberId == memberId
                             && t.OTPNumber == OTP
                             && t.OTPGenerateDateTime >= minDateTime && t.Status == "Active")
                    .FirstOrDefaultAsync();
                //var member = await uow.MemberRepository.Get().Where(t =>t.MemberId == memberId && t.OTPNumber == OTP &&  t.OTPGenerateDateTime.Value >= DateTimeHelper.DubaiTime().AddMinutes(-5)).FirstOrDefaultAsync();
                if (member != null)
                {
                    return member;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        public async Task UpdateFireBase(int MemberId, string FireBaseToken)
        {
            try
            {
               

                var result =await  uow.MemberFireBaseRepository.Get().Where(s => s.MemberId == MemberId).FirstOrDefaultAsync();


                //var result = db.MemberFirebases.Where(s => s.MemberId == MemberId).FirstOrDefault();

                if (result != null)
                {
                    result.FirebaseToken = FireBaseToken;
                    result.LastUpdated = DateTimeHelper.DubaiTime();

                    await uow.MemberFireBaseRepository.Edit(result);

                    //db.Entry(result).State = EntityState.Modified;
                    //db.SaveChanges();
                }
                else
                {
                    var fire = new MemberFirebase()
                    {
                        MemberId= MemberId,
                        FirebaseToken = FireBaseToken,
                        LastUpdated = Helper.DateTimeHelper.DubaiTime(),
                        Status="Active"

                    };

                    await uow.MemberFireBaseRepository.Add(fire);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        /// <summary>                            
        /// Member Details by Member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="lang"></param>
        /// <returns></returns>



        public async Task<Member> GetById(int memberId)

        {
            try
            {
                var db = new BoulevardDbContext();


                var memberResponse = await uow.MemberRepository.GetById(memberId);

                if (memberResponse != null) 
                {
                    var startdatetimeadded = Helper.DateTimeHelper.DubaiTime();
                    var subscription = await uow.MemberSubscriptionRepository.Get().Where(s => s.MemberId == memberId && s.Status == "Active" && s.StartDate<= startdatetimeadded  && s.EndDate>= startdatetimeadded).Include(s=>s.MemberShip).FirstOrDefaultAsync();
                    if (subscription != null)
                    { 
                        memberResponse.IsAnyMembership = true;
                        memberResponse.MemberSubscriptions = subscription;
                    }
                    var currentDate = Helper.DateTimeHelper.DubaiTime();
                    var currentMonth = currentDate.Month;
                    var currentYear = currentDate.Year;

                    memberResponse.MonthlyGoalAchivedAmount =
                       (db.OrderRequestService
                            .Where(t =>
                                t.BookingDate.Month == currentMonth &&
                                t.BookingDate.Year == currentYear)
                            .Sum(t => (double?)t.TotalPrice) ?? 0
                        +
                        db.OrderRequestProductss
                            .Where(t =>
                                t.OrderDateTime.Month == currentMonth &&
                                t.OrderDateTime.Year == currentYear)
                            .Sum(t => (double?)t.TotalPrice) ?? 0);
                    //memberResponse.MonthlyGoalAchivedAmount = db.OrderRequestService.Where(t => t.BookingDate.Date.Month == Helper.DateTimeHelper.DubaiTime().Month).Sum(t => t.TotalPrice) + db.OrderRequestProductss.Where(t => t.OrderDateTime.Date.Month == Helper.DateTimeHelper.DubaiTime().Month).Sum(t => t.TotalPrice);
                }
               
                
                return memberResponse;
            }
            catch (Exception ex)
            {
                //Log.Error(ex.ToString());
                return null;
            }
        }




    

        /// <summary>
        ///  Is Phone Number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public async Task<bool> IsPhoneNumber(string phoneNumber)
        {
            try
            {
                var member = await uow.MemberRepository.Get().Where(t => t.PhoneNumber == phoneNumber && t.Status == "Active").FirstOrDefaultAsync();
                return member != null ? true : false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }

        public async Task<Member> IsMailExist(string email)
        {

            return await uow.MemberRepository.GetIQueryable().FirstOrDefaultAsync(e => e.Email == email && e.Status == "Active");
        }



        public async Task<string> SendEmailForForgetPassword(string email)
        {
            var member = await IsMailExist(email);
            if (member != null)
            {

                var OTP = RandomStringHelper.RandomInteger(4);

                member.OTPNumber = OTP.ToString();


                await uow.MemberRepository.Edit(member);
                await new EmailService().ForgetPasswordEmail(member);
                return "An OTP is sent to your Email account.";
            }
            else
            {
                return "Sorry!!! Email is not Exist.";
            }
        }



        /// <summary>
        /// Is Email Number
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> IsEmailNumber(string email)
        {
            try
            {
                var member = await uow.MemberRepository.Get().Where(t => t.Email == email && t.Status == "Active").FirstOrDefaultAsync();
                return member != null ? true : false;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }

        public async Task<Member> IsEmailmember(string email)
        {
            try
            {
                var member = await uow.MemberRepository.Get().Where(t => t.Email == email && t.Status == "Active").FirstOrDefaultAsync();
               
                    return member;
                
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
        }

        /// <summary>
        ///  Profile Edit
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<Member> ProfileEdit(MemberRequest member)
        {
            try
            {
                var memberdetails = await uow.MemberRepository.GetById(member.MemberId);
                memberdetails.Name = member.Name;
              
                memberdetails.Email = member.Email;
                memberdetails.PhoneNumber =  member.PhoneNumber;
              
               
               
                


                if (!string.IsNullOrEmpty(member.Image))
                {
                    memberdetails.Image = member.Image;
                }

                memberdetails.UpdateBy = member.MemberId;
                memberdetails.UpdateDate = DateTimeHelper.DubaiTime();
                var mem = await uow.MemberRepository.Edit(memberdetails);
                return await GetById(Convert.ToInt32(mem.MemberId));
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }

        }



        public async Task UpdateMemberPassword(int memberId, string newPassword)
        {

            string PasswordHash = HashConfig.GetHash(newPassword);

            var member = await uow.MemberRepository.Get().Where(x => x.MemberId == memberId && x.Status == "Active").FirstOrDefaultAsync();

            if (member != null)
            {
                member.Password = PasswordHash;
                member.UpdateDate = DateTime.UtcNow;

                await uow.MemberRepository.Edit(member);
            }
        }


        public async Task<Member> IsMemberExist(int memberId)
        {

            return await uow.MemberRepository.Get().FirstOrDefaultAsync(e => e.MemberId == memberId && e.Status == "Active");
        }

        public async Task<bool> DeleteMember(int memberId)
        {
            try
            {
                // Fetch active member only
                var member = await uow.MemberRepository.Get()
                                .Where(x => x.MemberId == memberId && x.Status == "Active")
                                .FirstOrDefaultAsync();

                if (member != null)
                {
                    member.Status = "Deleted";               // Soft delete
                    member.UpdateDate = DateTime.UtcNow;    // Add timestamp

                    await uow.MemberRepository.Edit(member);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}