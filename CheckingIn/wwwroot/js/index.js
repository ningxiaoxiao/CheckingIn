/**
 * Created by Administrator on 2017/4/17.
 */
$('#look').click(function() {
    self.location = 'look.html?'+$('#formlook').serialize();
    return false;
});

$('#changepassword').click(function() {
    var name=$('input[name="cname"]').val();
    var pw=$('input[name="cpassword"]').val();
    var newpw=$('input[name="newpassword"]').val();

    $.get("changepw?name=" + name+"&pw="+pw+"&newpw="+newpw, function (data, status) {
        alert(data);
    });
    return false;
});


$('.form').find('input, textarea').on('keyup blur focus', function (e) {

    var $this = $(this),
        label = $this.prev('label');

    if (e.type === 'keyup') {
        if ($this.val() === '') {
            label.removeClass('active highlight');
        } else {
            label.addClass('active highlight');
        }
    } else if (e.type === 'blur') {
        if( $this.val() === '' ) {
            label.removeClass('active highlight');
        } else {
            label.removeClass('highlight');
        }
    } else if (e.type === 'focus') {

        if( $this.val() === '' ) {
            label.removeClass('highlight');
        }
        else if( $this.val() !== '' ) {
            label.addClass('highlight');
        }
    }

});

$('.tab a').on('click', function (e) {

    e.preventDefault();

    $(this).parent().addClass('active');
    $(this).parent().siblings().removeClass('active');

    target = $(this).attr('href');

    $('.tab-content > div').not(target).hide();

    $(target).fadeIn(600);

});