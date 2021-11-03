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
        }
    });
}