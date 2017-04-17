/**
 * Created by Administrator on 2017/4/17.
 */
$('#look').click(function() {
    var name = $('#name').val();
    if (name != '') {
        self.location = 'look.html?name=' + name;
    }
})