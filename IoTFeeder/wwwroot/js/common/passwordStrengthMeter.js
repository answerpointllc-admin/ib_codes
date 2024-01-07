function passwordStrengthForResetPassword(n){return n.toString().length<3?"":(score=0,n.length<6)?shortPass:(score+=n.length*4,score+=(checkRepetition(1,n).length-n.length)*1,score+=(checkRepetition(2,n).length-n.length)*1,score+=(checkRepetition(3,n).length-n.length)*1,score+=(checkRepetition(4,n).length-n.length)*1,n.match(/(.*[0-9].*[0-9].*[0-9])/)&&(score+=5),n.match(/(.*[!,@,#,$,%,^,&,*,?,_,~].*[!,@,#,$,%,^,&,*,?,_,~])/)&&(score+=5),n.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)&&(score+=10),n.match(/([a-zA-Z])/)&&n.match(/([0-9])/)&&(score+=15),n.match(/([!,@,#,$,%,^,&,*,?,_,~])/)&&n.match(/([0-9])/)&&(score+=15),n.match(/([!,@,#,$,%,^,&,*,?,_,~])/)&&n.match(/([a-zA-Z])/)&&(score+=15),(n.match(/^\w+$/)||n.match(/^\d+$/))&&(score-=10),score<0&&(score=0),score>100&&(score=100),score<34)?badPass:score<68?goodPass:strongPass}function passwordStrength(n,t){return n.toString().length<3?"":(score=0,n.length<6)?shortPass:n.toLowerCase()==t.toLowerCase()?badPass:(score+=n.length*4,score+=(checkRepetition(1,n).length-n.length)*1,score+=(checkRepetition(2,n).length-n.length)*1,score+=(checkRepetition(3,n).length-n.length)*1,score+=(checkRepetition(4,n).length-n.length)*1,n.match(/(.*[0-9].*[0-9].*[0-9])/)&&(score+=5),n.match(/(.*[!,@,#,$,%,^,&,*,?,_,~].*[!,@,#,$,%,^,&,*,?,_,~])/)&&(score+=5),n.match(/([a-z].*[A-Z])|([A-Z].*[a-z])/)&&(score+=10),n.match(/([a-zA-Z])/)&&n.match(/([0-9])/)&&(score+=15),n.match(/([!,@,#,$,%,^,&,*,?,_,~])/)&&n.match(/([0-9])/)&&(score+=15),n.match(/([!,@,#,$,%,^,&,*,?,_,~])/)&&n.match(/([a-zA-Z])/)&&(score+=15),(n.match(/^\w+$/)||n.match(/^\d+$/))&&(score-=10),score<0&&(score=0),score>100&&(score=100),score<34)?badPass:score<68?goodPass:strongPass}function checkRepetition(n,t){for(res="",i=0;i<t.length;i++){for(repeated=!0,j=0;j<n&&j+i+n<t.length;j++)repeated=repeated&&t.charAt(j+i)==t.charAt(j+i+n);j<n&&(repeated=!1);repeated?(i+=n-1,repeated=!1):res+=t.charAt(i)}return res}var shortPass="&nbsp;&nbsp;Too short",badPass="&nbsp;&nbsp;Average",goodPass="&nbsp;&nbsp;Good",strongPass="&nbsp;&nbsp;Strong"