

$('#btnSubmit').on('click', function () {

    if ($("#formAnswer").valid()) {
    } else
    {
        alert("提交失败，请按页面提示，将问卷填写完整后再提交！");
        $("#formAnswer").submit();
        return;
    }

    var lstQuestionRecordDetail = [];

    $('input:radio:checked').each(function () {

        lstQuestionRecordDetail.push({ QuestionnaireID: const_QuestionnaireID, QuestionID: $(this).attr("questionid"), QuestionOptionID: $(this).val(), OptionValue: '' });
        //var checkValue = $(this).val();
        console.log($(this).val());　　// 选中框中的值
    });

    $('input:checkbox:checked').each(function () {
        //var checkValue = $(this).val();
        lstQuestionRecordDetail.push({ QuestionnaireID: const_QuestionnaireID, QuestionID: $(this).attr("questionid"), QuestionOptionID: $(this).val(), OptionValue: '' });
        console.log($(this).val());　　// 选中框中的值
    });

    $('input:text').each(function () {
        //var checkValue = $(this).val();
        if ($(this).val() != ""){
            lstQuestionRecordDetail.push({ QuestionnaireID: const_QuestionnaireID, QuestionID: $(this).attr("questionid"), QuestionOptionID: $(this).attr("QuestionOptionID"), OptionValue: $(this).val() });
        }
        console.log($(this).val());　　// 选中框中的值
    });


    var QuestionRecord = {
        QuestionnaireID: const_QuestionnaireID,
        StatusFlags: const_StatusFlags
    };

    $.ajax({
        url: '/QuestionRecords/SaveQuestionRecord',
        type: 'post',//换成 get 无效
        contentType: 'application/json',
        data: JSON.stringify({
            questionRecord: QuestionRecord,
            lstQuestionRecordDetail: lstQuestionRecordDetail
        }),
        success: function (data) {
            alert("提交成功，感谢您的参与！");
        },
        error: function (data) {
            alert("网络连接失败，请切换网络后重试！");
        }
    });

});

/*

//表单验证
$(function () {
    //让当前表单调用validate方法，实现表单验证功能
    $("#formAnswer").validate({
        debug: true, //调试模式，即使验证成功也不会跳转到目标页面
        rules: {     //配置验证规则，key就是被验证的dom对象，value就是调用验证的方法(也是json格式)
            //text_155: {
            //    required: true,  //必填。如果验证方法不需要参数，则配置为true
            //    rangelength: [4, 30]
            //},
            //text_266: {
            //    required: true,
            //    rangelength: [2, 10]
            //},
            //text_267: {
            //    required: true,
            //    rangelength: [2, 15]
            //},
            //text_156: {
            //    required: true,
            //    rangelength: [11, 11]
            //},
            //text_157: {
            //    required: true,
            //    email: true
            //},
            radio_group_1: {
                required: true,
            },
            check_group_45: {
                required: true,
            },
        },
        messages: {
            //text_155: {
            //    required: "请输入您所在医院的名称",
            //    rangelength: $.validator.format("医院的名称长度必须在：{0}-{1}之间")
            //},
            //text_266: {
            //    required: "请输入您所在省的名称",
            //    rangelength: $.validator.format("省的名称长度必须在：{0}-{1}之间")
            //},
            //text_267: {
            //    required: "请输入您所在市的名称",
            //    rangelength: $.validator.format("市的名称长度必须在：{0}-{1}之间")
            //},
            //text_156: {
            //    required: "请输入您的手机号",
            //    rangelength: $.validator.format("手机号的长度必须为11位")
            //},
            //text_157: {
            //    required: "请输入您的E-mail",
            //    email: "邮箱格式不正确"
            //},
            radio_group_1: {
                required: "该题为必填项",
            },
            check_group_45: {
                required: "该题为必填项",
            },
        },
        errorPlacement: function (error, element) { //指定错误信息位置
            if (element.is(':radio') || element.is(':checkbox')) { //如果是radio或checkbox
                var eid = element.attr('name'); //获取元素的name属性
                error.insertBefore(element.parent().parent()); //将错误信息添加当前元素的父结点后面
            } else {
                error.insertAfter(element);
            }
        }
    });
});

*/