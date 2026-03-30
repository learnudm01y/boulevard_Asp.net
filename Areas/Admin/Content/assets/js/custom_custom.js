/**
 * Created by royex technologies on 12/3/2019.
 */

$(function () {
   var filterToggle = $('.filter-toggle'),
       filterHolder = $('.filter-holder');

   if(filterToggle.length>0){
       filterToggle.click(function () {
           filterHolder.slideToggle(300);
           var hasToggled = $(this).hasClass('filter-toggled');
           if(hasToggled==true){
               setTimeout(function () {
                   filterToggle.toggleClass('filter-toggled');
               },300);
           }else{
               filterToggle.toggleClass('filter-toggled');
           }
       });

       $('.cancel-filter--js').click(function () {
           filterHolder.slideToggle(300);
           setTimeout(function () {
               filterToggle.toggleClass('filter-toggled');
           },300);
       });
   }

    var changeLink = $('.change-link');
    if(changeLink.length>0){
        changeLink.click(function () {
            $(this).next().slideToggle('300');
            var clickedLink = $(this);
            var hasToggled = $(this).hasClass('change-link-toggled');
            if(hasToggled==true){
                setTimeout(function () {
                    clickedLink.toggleClass('change-link-toggled');
                },300);
            }else{
                clickedLink.toggleClass('change-link-toggled');
            }

        });
    }

   // datepicker
    var datepicker = $('.datepicker');
   if($('.datepicker').length>0){
       $('.datepicker').datepicker();
   }

   // js tree
   //  var tree = $('.tree--js');
   // tree.jstree();




    var categoryList = $('.category-list'),
        rootMenu = $('.root-menu'),
        menuLink = $('.category-list .menu-link'),
        innerMenu = $('.inner-menu');

   // if(categoryList.length>0){
   //     menuLink.click(function () {
   //         $(this).next().slideToggle('slow');
   //         $(this).toggleClass('active');
   //     });
   // }

   var sellerCategory = $('.seller-category'),
       toggleSign = $('.seller-category .menu-link .toggle-sign');
   if(sellerCategory.length>0){
       toggleSign.click(function () {
           $(this).parent().next().slideToggle('slow');
           $(this).parent().toggleClass('active');
       });
   }





   // accordion
    var accordionContainer = $('.accordion-container'),
        accordionContainerAllClosed = $('.accordion-container-all-closed');
   if($('.accordion-container').length>0){
       var accordion = new Accordion('.accordion-container',{
           showItem:true,
           itemNumber:0
       });
   }

   if(accordionContainerAllClosed.length>0){
       var accordion = new Accordion('.accordion-container-all-closed',{
           showItem:false,
           // itemNumber:0
       });
   }



   // dropify
    if($('.dropify').length >0){
        $('.dropify').dropify();
    }

    // select 2
    if($('.select-2').length>0){
        $('.select-2').select2();
    }

    if($('.select-searchless').length>0){
        $('.select-searchless').select2({
            minimumResultsForSearch: -1
        });
    }

    if($('[data-toggle="tooltip"]').length>0){
        $('[data-toggle="tooltip"]').tooltip();
    }

    //        tinymce editor
    if($('.description').length>0){
        tinymce.init({
            selector: "textarea.description",
            theme: "modern",
            height: 300,
            plugins: [
                "advlist autolink link image lists charmap print preview hr anchor pagebreak spellchecker",
                "searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking",
                "save table contextmenu directionality emoticons template paste textcolor"
            ],
            toolbar: "insertfile undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | l      ink image | print preview media fullpage | forecolor backcolor emoticons",

        });
    }

    // vertical tabs
    var verticalTabs = $('.v-tabs--js');
    if(verticalTabs.length>0){
        verticalTabs.tabs();
    }

    // tooltip
    var tooltip = $('.tool-tip');
    if(tooltip.length>0){
        $('[data-toggle="tooltip"]').tooltip();
    }

    // top buttons fixing
    var topButtons = $('.top-buttons');
    if(topButtons.length>0){
        var topbarHeight = $('.topbar').height(),
            topButtonsHeight = $('.top-buttons').height(),
            totalTop = topbarHeight + topButtonsHeight;
        var waypoint = new Waypoint({
            element: document.getElementById('new-operator'),
            handler: function(direction) {
                if(direction=='down'){
                    $('#new-operator').css({
                        'position':'fixed',
                        'top':topbarHeight,
                        'left':0,
                        'right':0,
                        'z-index':9,
                        'box-shadow':'0 1px 5px rgba(0, 0, 0, 0.05)'
                    });
                    $('.vertical-tab').css('margin-top',totalTop);
                }
                if(direction=='up'){
                    $('#new-operator').css({
                        'position':'static',
                        'top':topbarHeight,
                        'left':0,
                        'right':0,
                        'z-index':999,
                        'box-shadow':'0 1px 5px rgba(0, 0, 0, 0.05)'
                    });
                    $('.vertical-tab').css('margin-top',0);
                }

            },
            offset:topbarHeight
        });
    }

    var weightCheck = $('.weight-check');
    if(weightCheck.length>0){
        var checkBox = $('.lever');
        checkBox.click(function () {
            var status =  $(this).prev().prop('checked');
            if(status===true){
                $(this).parent().next().addClass('readonly');
                $(this).parent().next().attr('readonly','');
            }else{
                $(this).parent().next().removeClass('readonly');
                $(this).parent().next().removeAttr('readonly');
            }
        });
    }


    if($('#date-range').length>0){
        $('#date-range').datepicker({
            toggleActive: true
        });
    }



    if($('.weight-type').length>0){
        $('.weight-type').change(function () {
            $(this).val();
            if($(this).val()=='no weight'){
                $(this).parent().prev().find('.form-control').addClass('disabled');

            }
        });
    }




    // table editing
    $('.edit-column').click(function () {
        var $self = $(this);
        var value = parseInt($self.find('.value-holder').text());
        $self.find('.table-input').val(value);
        $self.find('.edit-column-value').hide();
        $self.find('.table-input').show().focus();
    });

    $('.table-input').blur(function () {
        var $self = $(this);
        $self.hide();
        $self.prev().find('.value-holder').text($self.val());
        $self.prev().show();
    });

    // if($('.prev-page').length>0){
    //     $('.prev-page').click(function () {
    //         parent.history.back();
    //         return false;
    //     });
    // }


    //        sweet alert
    $(".sa-delete").click(function () {
        const swalWithBootstrapButtons = Swal.mixin({
            customClass: {
                confirmButton: 'btn btn-success',
                cancelButton: 'mr-2 btn btn-danger'
            },
            buttonsStyling: false,
            animation:false,
            allowOutsideClick: false
        })

        swalWithBootstrapButtons.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Yes, delete it!',
            cancelButtonText: 'No, cancel!',
            reverseButtons: true
        }).then((result) => {
            if (result.value) {
            swalWithBootstrapButtons.fire(
                'Deleted!',
                'Your file has been deleted.',
                'success',

            )
//                do anything after succes
            console.log('success');
        }else{
                console.log('cancelled');
        }
    })
    });

    // datepicker icon click
    if($('.datepicker-holder').length>0){
        $('.datepicker-holder .input-group-text').click(function () {
            var self = $(this);
            self.closest('.input-group').find('input').focus();
        });
    }

    // back button
    $('.back-btn--js').click(function () {
        parent.history.back();
    });

    // timepicker
    if($('.timepicker').length>0){
        $('.timepicker').timepicker();
    }
    $('.timepicker-holder .input-group-text').click(function () {
        var self = $(this);
        self.closest('.input-group').find('input').focus();
    });

    // checkbox function in table
    $('.table .check-all').change(function () {
        var self = $(this),
            status = self.is(':checked');
        if(status){
            self.closest('.table').find('.check').prop('checked',true);
        }else{
            self.closest('.table').find('.check').prop('checked',false);
        }
    });

    $('.table .check').change(function () {
        var self = $(this),
            checkedLength = self.closest('.table').find('.check:checked').length,
            alLength = self.closest('.table').find('.check').length;
        if(checkedLength === alLength){
            self.closest('.table').find('.check-all').prop('checked',true);
        }else{
            self.closest('.table').find('.check-all').prop('checked',false);
        }
    });

    // scrolling for table
    function scrollTable(tableHolder,sign){
        var scrollableArea = tableHolder.find('.table').outerWidth() - tableHolder.outerWidth();

        $(tableHolder.find('.table-responsive')).animate({
            scrollLeft: ""+sign+"="+scrollableArea+"px"
        },'300');
    }

    $('.scroll-right').click(function () {
        var tableHolder = $(this).closest('.table-holder'),
            sign = "+";
        scrollTable(tableHolder,sign);
    });

    $('.scroll-left').click(function () {
        scrollTable($(this).closest('.table-holder'),"-");
    });

    $('.table-holder .table-responsive').on('scroll',function (e) {
        var self = $(this),
            scrollLeft = e.currentTarget.scrollLeft;
            // console.log(scrollLeft);
        if(scrollLeft >= self.find('.table').outerWidth() - self.outerWidth()){
            self.parent().find('.scroll-right').hide();
            self.parent().find('.scroll-left').show();
        }else{
            self.parent().find('.scroll-right').show();
            self.parent().find('.scroll-left').show();
        }
        if(!scrollLeft){
            self.parent().find('.scroll-right').show();
            self.parent().find('.scroll-left').hide();
        }
    });

    if($('.table-holder').length>0){
        var tableWidth = $('.table-holder .table').outerWidth(),
            tableHolderWidth = $('.table-holder .table-responsive').outerWidth(),
            widthDifference = tableWidth - tableHolderWidth;

        if(widthDifference == 0){
            $('.table-holder').find('.scroll-right').hide();
            $('.table-holder').find('.scroll-left').hide();
        }
    }


    // currency symbols
    $('.symbol-holder input[type=checkbox]').each(function () {
        var self = $(this),
            isChecked = self.is(':checked');
        if(isChecked){
            self.closest('.symbol-holder').find('input[type=text]').addClass('disabled');
            self.closest('.symbol-holder').find('input[type=text]').attr('readonly','');
        }
    });

    $('.symbol-holder input[type=checkbox]').change(function () {
        var self = $(this),
            isChecked = self.is(':checked');
        if(isChecked){
            self.closest('.symbol-holder').find('input[type=text]').addClass('disabled');
            self.closest('.symbol-holder').find('input[type=text]').attr('readonly','');
        }else{
            self.closest('.symbol-holder').find('input[type=text]').removeClass('disabled');
            self.closest('.symbol-holder').find('input[type=text]').removeAttr('readonly');
        }
    });


    // validation
    // decimal number validation
    $(".decimal-field").keypress(function(e) {
        var self = $(this);
        var val = $(this).val();
        var regex = /^(\+|-)?(\d*\.?\d*)$/;
        if (regex.test(val + String.fromCharCode(e.charCode))) {
            return true;
        }else{
            if(self.parent().find('.error-msg').length>0){
                return false;
            }else{
                self.parent().append('<p class="error-msg text-danger">Only Decimal Value is allowed!</p>');
                setTimeout(function () {
                    self.parent().find('.error-msg').fadeOut().remove();
                },1000);
            }
        }
        return false;
    });

    // numeric nuber validation
    $(".numeric-field").keypress(function(e) {
        var self = $(this);
        var val = $(this).val();
        var regex = /^[0-9]*$/;
        if (regex.test(val + String.fromCharCode(e.charCode))) {
            return true;
        }else{
            if(self.parent().find('.error-msg').length>0){
                return false;
            }else{
                self.parent().append('<p class="error-msg text-danger">Only Numerical Value is allowed!</p>');
                setTimeout(function () {
                    self.parent().find('.error-msg').fadeOut().remove();
                },1000);
            }
        }
        return false;
    });


    if($('.api-sidebar').length>0){
        $('.api-list.active .collapse').addClass('in');
        $('.api-sidebar a.has-arrow').click(function () {
            var self = $(this);
            self.closest('#documentaion-nav').find('.api-list').removeClass('active');
            self.closest('#documentaion-nav').find('.api-list').find('.collapse').removeClass('in');
            self.parent().toggleClass('active');
            self.next().toggleClass('in');
        });
    }

    // custom scrolllbar
    var Scrollbar = window.Scrollbar;
    if (Scrollbar) {
        Scrollbar.initAll({
            damping:'0.05',
        });
    }

    // schedule-swiper
    if (typeof Swiper !== 'undefined' && document.querySelector('.schedule-swiper--js')) {
        var scheduleSwiper = new Swiper('.schedule-swiper--js',{
            speed:400,
            spaceBetween:10,
            slidesPerView:4,
            resistance:true,
            resistanceRatio:0.01
        });
    }


});

