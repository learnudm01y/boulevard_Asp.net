using Boulevard.Helper;
using Boulevard.RequestModels;
using Boulevard.Service.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using System.Web.Services.Description;

namespace Boulevard.Controllers
{
    public class MemberController : BaseController
    {
        private MemberServiceAccess _memberService;
        public MemberController()
        {
            _memberService = new MemberServiceAccess();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Register(MemberRequest model)
        {
            if (!string.IsNullOrEmpty(model.Email))
            {
                var isExist = await _memberService.IsEmailNumber(model.Email);
                if (isExist)
                {
                    return ErrorMessageNull("Email already exists, please login!",null);
                }
            }
            if (!string.IsNullOrEmpty(model.PhoneNumber))
            {
                var isExist = await _memberService.IsPhoneNumber(model.PhoneNumber);
                if (isExist)
                {
                    return ErrorMessageNull("Mobile Number already exists, please login!", null);
                }
            }
            var customer = await _memberService.Registration(model);


            #region Send Mail
            if (customer != null)
            {

                //await new EmailService().SendRegistrationResponse(customer);
                return SuccessMessage(customer);
            }
            else
            {
                return BadRequest("some problem occured");
            }
            #endregion
           

        }

        [HttpPost]
        public async Task<IHttpActionResult> LoginWithEmail(MemberLogin model)
        {
            if (model == null)
                return ErrorMessageNull("UserName is required!", null);

            var member = await _memberService.Login(model);

            if (member == null)
            {
                return ErrorMessageNull("UserName & Password is Incorrect.", null);
            }

            if (member.MemberId > 0)
            {
                if (member.Status == "Active")
                {
                    member.ThirdPartyLogin = false;
                    return SuccessMessage(member);
                }
                else
                {
                    return ErrorMessageNull("Please verify your email id to login to the profile", null);
                }


            }
            else if (member.MemberId == 0)
            {
                return ErrorMessageNull("UserName & Password is Incorrect.", member);
            }
            else
            {
                return InternelServerError();
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> RegistrationFromThirdParty(MemberRequest model)
        {



            if (!string.IsNullOrEmpty(model.ThirdPartyLoginKey) && !string.IsNullOrEmpty(model.ThirdPartyLoginFrom) && !string.IsNullOrEmpty(model.Email))
            {
                var loginMember = new Models.Member();
                var memberLogin = await _memberService.IsEmailmember(model.Email);
                if (memberLogin != null)
                {

                    loginMember = await _memberService.LoginForThirdPartyAsync(Convert.ToInt32(memberLogin.MemberId), model.MemberFireBaseToken, model.ThirdPartyLoginKey, model.ThirdPartyLoginFrom);

                    if (loginMember != null)
                    {
                        loginMember.ThirdPartyLogin = true;
                        return SuccessMessage(loginMember);
                    }
                }
                else
                {
                    var registeredMember = await _memberService.RegisterThirdPartyAsync(model);
                    if (registeredMember != null)
                    {
                        registeredMember.ThirdPartyLogin = true;


                        return SuccessMessage(loginMember);
                    }
                    else
                    {
                        return ErrorMessage("An error occured, please try again later!");

                    }
                }
            }
            return ErrorMessage("An error occured, please try again later!");


        }

        [HttpPost]
        public async Task<IHttpActionResult> Login(MemberLogin model)
        {
            if (model == null)
                return ErrorMessageNull("Mobile Number is required!", null);

            var member = await _memberService.LoginForOTP(model);

            if (member == null)
            {
                return ErrorMessageNull("Mobile Number is Incorrect.", null);
            }

            if (member.MemberId > 0)
            {
                if (member.Status == "Active")
                {
                    return SuccessMessage(member);
                }
                else
                {
                    return ErrorMessageNull("Please verify your email id to login to the profile", null);
                }


            }
            else if (member.MemberId == 0)
            {
                return ErrorMessageNull("UserName & Password is Incorrect.", member);
            }
            else
            {
                return InternelServerError();
            }
        }


        [HttpGet]
        public async Task<IHttpActionResult> OtpCheck(string Otp, long memberId)
        {
            if (string.IsNullOrEmpty(Otp))
            {
                return ErrorMessage("Please Enter OTP.");
            }
            var member = await _memberService.OTPCheck(Otp, memberId);
            if (member == null)
            {
                return ErrorMessageNull("The OTP you entered is incorrect or has expired. Please try again.", null);
            }
            return SuccessMessage(member);
        }


        [HttpGet]
        public async Task<IHttpActionResult> MemberDetails(int id)
        {
            if (id == 0)
            {
                return ErrorMessage("Please Enter Id.");
            }
            var member = await _memberService.GetById(id);
            return SuccessMessage(member);
        }


        [HttpGet]
        public async Task<IHttpActionResult> MemberDetailsV2(int id)
        {
            if (id == 0)
            {
                return ErrorMessage("Please Enter Id.");
            }
            var member = await _memberService.GetById(id);
            return SuccessMessage(member);
        }

        [HttpGet]
        public async Task<IHttpActionResult> MemberDelete(int memberId)
        {
            if (memberId == 0)
            {
                return ErrorMessage("Please Enter Id.");
            }
            var ss =  await _memberService.DeleteMember(memberId);
            if (ss == true)
            {
                return SuccessMessage(ss);
            }
            else
            {
                return ErrorMessage("Some thing went wrong");
            }
          
        }

        [HttpPost]
        public async Task<IHttpActionResult> ProfileEdit(MemberRequest model)
        {
            if (model == null)
                return ErrorMessage("Name is required!");

            var member = await _memberService.ProfileEdit(model);
            if (member.MemberId > 0)
            {
                return SuccessMessage(member);
            }
            else
            {
                return InternelServerError();
            }
        }




        [HttpPost]
        public async Task<IHttpActionResult> MemberForgetPasswordV2(ForgetPasswordRequest model)
        {
            if (model.Email != "" && model.OTP == 0 && model.Password == "")
            {
                string message = await _memberService.SendEmailForForgetPassword(model.Email);

                return SuccessMessage(null,message);
            }
            else if (model.Email != "" && model.OTP != 0 && model.Password == "")
            {
                var member = await _memberService.IsMailExist(model.Email);

                if (member.OTPNumber == model.OTP.ToString())
                {
                    return SuccessMessage(null, "Verification Success");
                }
                else
                {
                    return ErrorMessageNull("Not Verified",model.OTP.ToString());
                }
            }
            else if (model.Email != "" && model.OTP == 0 && model.Password != "")
            {
                var member = await _memberService.IsMailExist(model.Email);

                if (member != null)
                {
                    await _memberService.UpdateMemberPassword(Convert.ToInt32(member.MemberId), model.Password);

                    return SuccessMessage(null);
                }
                else
                {
                    return ErrorMessageNull("Faild", member);
                }

            }
            else
            {
                return ErrorMessageNull("Faild",null);
            }
        }


        [HttpPost]
        public async Task<IHttpActionResult> UpdateMemberPassword(PasswordUpdateRequest model)
        {
            if (string.IsNullOrEmpty(model.OldPassword))
            {
                return ErrorMessage("Member password not correct, please try again!");
               
            }

            if (model.memberId > 0)
            {
                string oldHash = HashConfig.GetHash(model.OldPassword);
                var member = await new MemberServiceAccess().IsMemberExist(model.memberId);
                if (member == null)
                {
                    return ErrorMessage("Member not exists, please register!");
                    
                }
                else if (member.Password != oldHash)
                {
                    return ErrorMessageNull("Previous password not correct, please try again!",null);
                   
                }
            }
            else
            {
                return ErrorMessageNull("Member not valid, please try again!",null);

              
            }

            await new MemberServiceAccess().UpdateMemberPassword(model.memberId, model.Password);
            return SuccessMessage(null,"Password Updated successfully");
            
        }
    }
}
