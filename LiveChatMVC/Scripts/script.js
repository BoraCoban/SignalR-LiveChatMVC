// Prepairing UI
(function ($) {
    $(document).ready(function () {

        
        var $chatbox = $('.chatbox'),
            $chatboxTitle = $('.chatbox__title'),
            $chatboxTitleClose = $('.chatbox__title__close')
            $musicOff = $('.music-off')
            $eyeOff = $('.eye-off')
           
       
        $chatboxTitle.children().not('#music-off,#music-on,#eye-off,#eye-on,#arrow-left,#arrow-right').on('click', function () {
            $chatbox.toggleClass('chatbox--tray');
            unReadMessageCount = 0
            $('#message-alert').css("display", "none")
            $('#message-alert1').css("display", "none")
        });

       
        $chatboxTitleClose.on('click', function (e) {
            
            e.stopPropagation();
            $chatbox.toggleClass('chatbox--tray');
        })
    });

    $('#new-message-alert').click(function () {
        $('#chat_body').animate({ scrollTop: $('#chat_body')[0].scrollHeight }, 400)
    })

    $('#new-message-alert1').click(function () {
        $('#chat_body').animate({ scrollTop: $('#chat_body')[0].scrollHeight }, 400)
    })

   /* $('#lock').click(function () {
        $('#unlock').css("display", "block")
        $('#lock').css("display", "none")
        pushNotification = true;
    })

    $('#unlock').click(function () {
        $('#lock').css("display", "block")
        $('#unlock').css("display", "none")
        pushNotification = false;
    })*/

    $('#music-off').click(function () {
        $('#music-on').css("display", "block")
        $('#music-off').css("display", "none")
        mySound = true;
    })
    $('#music-on').click(function () {
        $('#music-off').css("display", "block")
        $('#music-on').css("display", "none")
        mySound = false;
    })
    $('#online').css("display", "none")
    $('#eye-off').click(function () {
        $('#eye-on').css("display", "block")
        $('#eye-off').css("display", "none")
        $('#online').css("display","none")
    })
    $('#eye-on').click(function () {
        $('#eye-off').css("display", "block")
        $('#eye-on').css("display", "none")
        $('#online').css("display", "block")
    })
    
    
    $('#arrow-left').click(function () {
        $('#arrow-right').css("display", "block")
        $('#arrow-left').css("display", "none")
        $('.chatbox').css("width", "500px")
        $('#more').css("right", "250px")
        $('#online').css("right", "525px")
    })

    $('#arrow-right').click(function () {
        $('#arrow-right').css("display", "none")
        $('#arrow-left').css("display", "block")
        $('.chatbox').css("width", "320px")
        $('#more').css("right", "150px")
        $('#online').css("right", "345px")
    })

})(jQuery);

// Variable for handling chatHub class.
var chat = $.connection.chatHub;
var unReadMessageCount = 0
var unReadMessageCountNow = 0
var mySound = true;
var pushNotification = true;
var i = 0;
// Adding message to ui. Must invoke from hub when someone send message
chat.client.addNewMessageToPage = function (name, message, creationDate) {

    unReadMessageCount++

    newMessageLeft(message, creationDate, name)

    if ($('#new-message-alert').is(':visible') == false) {
        $('#chat_body').scrollTop($('#chat_body')[0].scrollHeight)
    }
    if ($('#new-message-alert1').is(':visible') == false) {
        $('#chat_body').scrollTop($('#chat_body')[0].scrollHeight)
    }
    

    if ($('#chat-window').hasClass('chatbox--tray') && unReadMessageCount > 0) {
        $('#message-alert').css("display", "block")
        $('#message-alert1').css("display", "block")
        
        /*if (unReadMessageCount == 1) {
                if (pushNotification == true) {
                    alert("New Message");
                }
        }*/
        

        $('#message-count').html(unReadMessageCount)
        $('#message-count1').html(unReadMessageCount)

        playNotification('onClose')
    }

    else {
        
        unReadMessageCount = 0
        
    }


    if (($('#chat_body').scrollTop() - $('#chat_body')[0].scrollHeight) < 287) {
        unReadMessageCountNow++
        $('#unread-message-count').css("display", "block").html(unReadMessageCountNow)
        $('#unread-message-count1').css("display", "block").html(unReadMessageCountNow)
        playNotification()
    }
}

//Fetch history message and more button 

chat.client.printMessageHistory = function (message, userId) {
    window.msghIs = 40
    console.log(message);
    $('#more').on('click', function () {
        msghIs += 20
        for (var i = msghIs-20; i < msghIs; i++) {

            var element = message[i];

            if (!(userId == element.u.Id)) {
                newMessageLeftForMore(element.m.MessageBody, element.m.CreatedAt, element.u.Name)
            } else {
                newMessageRightForMore(element.m.MessageBody, element.m.CreatedAt, element.u.Name, false)
            }

        }
    });
    for (var i = 0; i < msghIs; i++) {

        var element = message[i];

        if (!(userId == element.u.Id)) {
            newMessageLeftForMore(element.m.MessageBody, element.m.CreatedAt, element.u.Name)
        } else {
            newMessageRightForMore(element.m.MessageBody, element.m.CreatedAt, element.u.Name, false)
        }

        $('#chat_body').scrollTop($('#chat_body')[0].scrollHeight);
    }

}

chat.client.userLeft = function (userName) {
    userLeftAlert(userName)
}

// Add new message to callerclient
chat.client.addNewMessageToMe = function (name, message, creationDate) {
    newMessageRight(message, creationDate, name, true)
    var validUrl = isURL(message.MessageBody)

    if (validUrl) {
        console.log('Bu bir link')
    }

}

function newMessageLeftForMore(MessageBody, CreatedAt, Name) {

    var stringHTML = ('<div><div class="mb-3"><div class="chatbox__body__message chatbox__body__message--left">'
        + '<div class="clearfix"></div>'
        + '<div class="ul_section_full">'
        + '<ul class="ul_msg">'
        + '<li class="message"></li>'
        + '</ul>'
        + '<div class="clearfix"></div>'
        + '</div>'
        + '</div>'
        + '<div style="font-size:9px;margin-top:4px" class="d-flex justify-content-between pr-5 pl-2"><i><i class="far fa-clock mr-1"></i>' + formatDate(CreatedAt) + '</i></i><span>' + Name + '</span></div>'
        + '</div>'
        + '</div>')

    var html = $.parseHTML(stringHTML)
    $(html).find("li.message").html($("<div>").text(MessageBody).html());
    var chatBody = $('#chat_body')

    chatBody.prepend(html)

}
//Safe New Message Functions
function newMessageLeft(MessageBody, CreatedAt, Name) {

    var stringHTML = ('<div><div class="mb-3"><div class="chatbox__body__message chatbox__body__message--left">'
        + '<div class="clearfix"></div>'
        + '<div class="ul_section_full">'
        + '<ul class="ul_msg">'
        + '<li class="message"></li>'
        + '</ul>'
        + '<div class="clearfix"></div>'
        + '</div>'
        + '</div>'
        + '<div style="font-size:9px;margin-top:4px" class="d-flex justify-content-between pr-5 pl-2"><i><i class="far fa-clock mr-1"></i>' + formatDate(CreatedAt) + '</i></i><span>' + Name + '</span></div>'
        + '</div>'
        + '</div>')

    var html = $.parseHTML(stringHTML);
    $(html).find("li.message").text($("<div>").text(MessageBody).html());
    var chatBody = $('#chat_body')

    chatBody.append(html)

}
function newMessageRightForMore(MessageBody, CreatedAt, Name, isMe) {
    var stringHTML = ('<div class="mb-3"><div class="chatbox__body__message chatbox__body__message--right">'
        + '<div class="clearfix"></div>'
        + '<div class="ul_section_full">'
        + '<ul class="ul_msg">'
        + '<li class="message"></li>'
        + '</ul>'
        + '<div class="clearfix"></div>'
        + '</div>'
        + '</div>'
        + '<i class="time-right d-flex justify-content-end">You-' + formatDate(CreatedAt) + '</i>'
        + '</div>')

    var html = $.parseHTML(stringHTML);
    $(html).find("li.message").text($("<div>").text(MessageBody).html());
    var chatBody = $('#chat_body')

    if (isMe) {
        chatBody.prepend(html).animate({ scrollTop: $('#chat_body')[0].scrollHeight }, 400)
    }
    else {
        chatBody.prepend(html)
    }
}


function newMessageRight(MessageBody, CreatedAt, Name, isMe) {
    var stringHTML = ('<div class="mb-3"><div class="chatbox__body__message chatbox__body__message--right">'
        + '<div class="clearfix"></div>'
        + '<div class="ul_section_full">'
        + '<ul class="ul_msg">'
        + '<li class="message"></li>'
        + '</ul>'
        + '<div class="clearfix"></div>'
        + '</div>'
        + '</div>'
        + '<i class="time-right d-flex justify-content-end">You-' + formatDate(CreatedAt) + '</i>'
        + '</div>')

    var html = $.parseHTML(stringHTML);
    $(html).find("li.message").html($("<div>").text(MessageBody).html());
    var chatBody = $('#chat_body')

    if (isMe) {
        chatBody.append(html).animate({ scrollTop: $('#chat_body')[0].scrollHeight }, 400)
    }
    else {
        chatBody.append(html)
    }
}

function userLeftAlert(userName) {
    var stringHTML = ('<div class="chatbox__body__message chatbox__body__message--left">'
        + '<div class="clearfix"></div>'
        + '<div class="ul_section_full_red">'
        + '<ul class="ul_msg d-flex justify-content-between">'
        + '<li class="message" style="font-size:10px">' + userName + " ayrıldı." + '</li>'
        + '<li class="timing"><span class="mr-2">' + new Date().toLocaleTimeString() + '</span><i class="fa fa-clock"></li>'
        + '</ul>'
        + '<div class="clearfix"></div>'
        + '</div>'
        + '</div>')

    var html = $.parseHTML(stringHTML)
    $('#chat_body').append(html)

}



function formatDate(date) {

    var newDate = new Date(date)
    var messageDate = moment(date, "YYYY-MM-DD");
    var current = moment().startOf('day');
    var diff = moment.duration(messageDate.diff(current)).asDays();

    var formattedDate = diff < 0 ? moment(newDate).fromNow() : moment(newDate).format("HH:mm")

    return formattedDate
}

// TODO: Show count of online users in chat.
chat.client.updateUserCount = function (count) {
    
    if (count < 2) {
        $('#title').addClass('gray_title')
        var html = $.parseHTML('<a href="#">Chat ( <i class="fas fa-user-tie pr-2"></i>' + (count ) + ' Online)</a>')
        $('#title').html(html)
    }
    else {
        $('#title').removeClass('gray_title')
        var html = $.parseHTML('<a href="#">Chat ( <i class="fas fa-user-tie pr-2"></i>' + (count ) + ' Online)</a>')
        $('#title').html(html)
    }
}


chat.client.updateUser = function (onlineUserList) {

    console.log("onlineUserList",onlineUserList);
    $('#online').empty();
    $.each(onlineUserList, function (key, value) {
            $('#online').append($('<li>' + value.UserName + '</li>'))
    });
};


// When hub started succesfully or fail.
$.connection.hub.start().done(function () {
    var token = prompt('Enter your Username : ', '');
    chat.server.connect(token);
 
    $('#btn-chat').on('click', function () {

        var value = $('#btn-input').val()
        if (value == '' || value == null) {
            console.log('Boş mesaj atılamaz!')
        }
        else {
            chat.server.send(value);
            $('#btn-input').val('');
            $('#btn-input').focus();
        }
    });
    var $chatBody = $('#chat_body')
    $chatBody.scroll(function () {
        if (($chatBody[0].scrollHeight) - $chatBody.scrollTop() > 287) {
            $('#new-message-alert').fadeIn('slow')
        }
        else {
            $('#new-message-alert').fadeOut('fast')
            $('#unread-message-count').css("display", "none")
            unReadMessageCountNow = 0
        }
    })

    $chatBody.scroll(function () {
        if ($chatBody.scrollTop() == 0) {
            $('#more').fadeIn('slow')
        }
        else {
            $('#more').fadeOut('fast')
            $('#unread-message-count').css("display", "none")
            unReadMessageCountNow = 0
        }
    })

}).fail(function () {
    console.log("Hub starting fail")
});




//Enter Trigger
$('#btn-input').keyup(function (e) {
    if (e.keyCode == 13) {
        $('#btn-chat').click()
    }
})

function isURL(str) {
    var pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
        '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.?)+[a-z]{2,}|' + // domain name
        '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
        '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
        '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
        '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
    return pattern.test(str);
}
function playNotification(type) {

    var audio = type == 'onClose' ? new Audio('https://notificationsounds.com/soundfiles/c9892a989183de32e976c6f04e700201/file-sounds-1109-slow-spring-board-longer-tail.mp3')
        : new Audio('https://notificationsounds.com/soundfiles/58ae749f25eded36f486bc85feb3f0ab/file-sounds-1094-intuition.mp3')

    if (mySound == true) {
        audio.play();
    }
}
