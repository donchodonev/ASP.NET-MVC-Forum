function sendVote(postId, isPositiveVote) {
    var token = $("#votesForm input[name=__RequestVerificationToken]").val();
    var json = { postId: postId, isPositiveVote: isPositiveVote };
    $.ajax({
        url: "/api/votes",
        type: "POST",
        data: JSON.stringify(json),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        headers: { 'X-CSRF-TOKEN': token },
        success: function (data) {
            $("#voteSum").html(data.voteSum);
            if (isPositiveVote) {
                $("#up")
                    .addClass("bi bi-hand-thumbs-up-fill")
                    .removeClass("bi bi-hand-thumbs-up");

                $("#down")
                    .addClass("bi bi-hand-thumbs-down")
                    .removeClass("bi bi-hand-thumbs-down-fill");
            }
            else {
                $("#up")
                    .addClass("bi bi-hand-thumbs-up")
                    .removeClass("bi bi-hand-thumbs-up-fill");

                $("#down")
                    .addClass("bi bi-hand-thumbs-down-fill")
                    .removeClass("bi bi-hand-thumbs-down");
            }
        },
        error: function (xhr) {
            if (xhr.status == 401) {
                var loginUrl = xhr.getResponseHeader("location");
                window.location.href = loginUrl;
            }
        }
    });
}