
$("button#sendComment").click(function (e) {
    e.preventDefault();
    var token = $("#commentsForm [name=__RequestVerificationToken]").val();
    var data = {
        CommentText: $("input#comment").val()
        , PostId: $("#PostId").val()
    };

    function postComment() {
        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            url: '/api/comments',
            data: JSON.stringify(data),
            dataType: "json",
            headers: { 'X-CSRF-TOKEN': token },
            success: function (returnData) {
                let commentId = returnData.id;
                let commentCountPlus1 = parseInt($("span#commentCount").text()) + 1;
                $("span#commentCount").text(commentCountPlus1);
                var el = document.getElementById('serviceDrop');
                $("input#comment").val(''),
                    $('.collapse').collapse('show')
                if (el.classList.contains('fa-chevron-up')) {
                    $('<div id="' + commentId + '"><div class="d-flex flex-row align-items-center commented-user"><h5 class="mb-1 mt-1">' + returnData.username + '<span class="dot mr-2 ml-2 mb-1 mt-1"></span>' + '<h6 class="font-weight-normal font-italic mt-2">' + returnData.createdOnAsString + '</h6>' + '</h5><div class="ml-auto"><i id="' + commentId + '" onclick="removeComment(this);" type="button" class="bi bi-trash"></i></div></div ><div class="comment-text-sm"><span>' + returnData.commentText + '</span></div>').prependTo('.commentsArray');
                }
            }
        });
    };

    postComment();
});


$(document).ready(function () {
    $('#collapse-1').on('shown.bs.collapse', function () {
        $.getJSON('/api/comments', { postId: $("#PostId").val() },
            function (comments) {
                for (var i = 0; i < comments.length; i++) {
                    var isAdmin = $("#isAdmin").val();
                    var currentUserUsername = $("#currentUserUsername").val();
                    var currentCommentId = comments[i].id;
                    var currentComment = (comments[i].content);
                    var currentCommentAuthor = comments[i].commentAuthor;
                    var createdOn = comments[i].createdOnAsString;
                    var trashButtonHtml = '';
                    debugger;
                    if (currentCommentAuthor == currentUserUsername || isAdmin == "True")
                    {
                        trashButtonHtml = '<i id="' + currentCommentId + '" onclick="removeComment(this);" type="button" class="bi bi-trash"></i>';
                    }

                    $('<div id="' + currentCommentId + '"><div class="d-flex flex-row align-items-center commented-user"><h5 class="mb-1 mt-1">' + currentCommentAuthor +
                        '<span class="dot mr-2 ml-2 mb-1 mt-1"></span>' + '<h6 class="font-weight-normal font-italic mt-2">' + createdOn +
                        '</h6>' + '</h5><div class="ml-auto">' + trashButtonHtml + '</div></div><div class="comment-text-sm"><span>' + currentComment + '</span></div></div>').appendTo('.commentsArray');
                }
            }
        )
        $(".servicedrop").addClass('fa-chevron-up').removeClass('fa-chevron-down');
    });
    $('#collapse-1').on('hidden.bs.collapse', function () {
        $(".servicedrop").addClass('fa-chevron-down').removeClass('fa-chevron-up');
        $("div.comment-text-sm").remove();
        $("div.commented-user").remove();
    });
});


function removeComment(comment) {
    var token = $("#commentsForm [name=__RequestVerificationToken]").val();
    var idData = $(comment).attr("id");
    $.ajax({
        type: 'DELETE',
        contentType: 'application/json',
        url: '/api/comments/' + idData,
        headers: { 'X-CSRF-TOKEN': token },
        success: function () {
            $("div#" + $(comment).attr("id").toString()).remove();
            let commentCountMinus1 = parseInt($("span#commentCount").text()) - 1;
            $("span#commentCount").text(commentCountMinus1);
        },
        error: function (httpObj, textStatus) {
            if (httpObj.status == 401) {
                alert(httpObj.responseText)
            }
        }
    });
}
