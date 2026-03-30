
//$(function () {


//    $("#CurrencyId").change(function () {
//        var id = $("#CurrencyId").val();
//        $("#storeViewModel.CurrencyId").val(id);
//    });


//    // Load State by Country Id
//    $("#CountryId").change(function () {
//        var id = $("#CountryId").val();
//        $.ajax({
//            url: "../Common/GetStatesbyCountryId/" + id,
//            type: "GET",
//            success: function (data) {
//                // Clean city
//                var city = $("#CityId");
//                city.empty();

//                var state = $("#StateId");
//                state.empty();
//                state.append($('<option/>',
//                    {
//                        value: "",
//                        text: "--Select State--"
//                    }));
//                $.each(data,
//                    function (index, itemData) {
//                        state.append($('<option/>',
//                            {
//                                value: itemData.StateId,
//                                text: itemData.StateName
//                            }));
//                    });
//            },
//            error: function (e) {
//                alert(JSON.stringify(e));
//            }
//        });
//    });


//    // Load City by State Id
//    $("#StateId").change(function () {
//        var id = $("#StateId").val();
//        //   alert(id);
//        $.ajax({
//            url: "../Common/GetCitysbyStateId/" + id,
//            type: "GET",
//            success: function (data) {
//                // Clean city
//                var select = $("#CityId");
//                select.empty();
//                select.append($('<option/>',
//                    {
//                        value: "",
//                        text: "--Select City--"
//                    }));
//                $.each(data,
//                    function (index, itemData) {
//                        select.append($('<option/>',
//                            {
//                                value: itemData.CityId,
//                                text: itemData.CityName
//                            }));
//                    });
//            },
//            error: function (e) {
//                alert(JSON.stringify(e));
//            }
//        });
//    });



//    $("#CurrencyId").change(function () {
//        var id = $("#CurrencyId").val();
//        $("#storeViewModel.CurrencyId").val(id)
//    });


//    $(document).on('change', '#appendDiv .countryId', function () {
//        var parent = $(this).parent().parent().parent();
//        var id = $(this).val();
//        $.ajax({
//            url: "../Common/GetStatesbyCountryId/" + id,
//            type: "GET",
//            success: function (data) {
//                // Clean city
//                var city = parent.find(".cityId");
//                city.empty();

//                var state = parent.find(".stateId");
//                state.empty();
//                state.append($('<option/>',
//                    {
//                        value: "",
//                        text: "--Select State--"
//                    }));
//                $.each(data,
//                    function (index, itemData) {
//                        state.append($('<option/>',
//                            {
//                                value: itemData.StateId,
//                                text: itemData.StateName
//                            }));
//                    });
//            },
//            error: function (e) {
//                alert(JSON.stringify(e));
//            }
//        });
//    });
//    // Load State by Country Id



//    // Load City by State Id
//    $(document).on('change', '#appendDiv .stateId', function () {


//        var parent = $(this).parent().parent().parent();
//        var id = $(this).val();
//        //   alert(id);
//        $.ajax({
//            url: "../Common/GetCitysbyStateId/" + id,
//            type: "GET",
//            success: function (data) {
//                // Clean city
//                var select = parent.find(".cityId");
//                select.empty();
//                select.append($('<option/>',
//                    {
//                        value: "",
//                        text: "--Select City--"
//                    }));
//                $.each(data,
//                    function (index, itemData) {
//                        select.append($('<option/>',
//                            {
//                                value: itemData.CityId,
//                                text: itemData.CityName
//                            }));
//                    });
//            },
//            error: function (e) {
//                alert(JSON.stringify(e));
//            }
//        });
//    });

//});