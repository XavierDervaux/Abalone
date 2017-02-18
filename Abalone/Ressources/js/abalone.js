/**
    Initialisation
**/

var MIN_MDP = 8;

function init(){
    $('[data-toggle="popover"]').popover();
    
    if(location.hash == "#inscription"){
       $('#contentConnexion a:last').tab('show');
    }
}

/**
    Fonctions setting
**/

function checkSetting(){
    var ok = true;
    
    if($id('emailSetting').value.length == 0){
        ok = false;
    } else if($id('passwordSetting').value.length > 0 || $id('confirmPasswordSetting').value.length > 0){
        ok =  checkPassword($id('passwordSetting'),$id('confirmPasswordSetting'), false);
    }
    if(ok && validateEmail($id('emailSetting'))){ 
         $id('submitSetting').disabled = false; 
    } else{
         $id('submitSetting').disabled = true; 
    }
}

function checkPasswordSetting(){
    checkPassword($id('passwordSetting'),$id('confirmPasswordSetting'), true);
     if($id('passwordSetting').value.length == 0 && $id('confirmPasswordSetting').value.length == 0){
           inputNeutre($id('passwordSetting'));
           inputNeutre($id('confirmPasswordSetting'));
    }
}

/**
   Fonctions Connexion
**/

function checkConnecion(){
    ok = true;
     if($id('emailConnection').value.length == 0){
       ok = false; 
    } else if($id('passwordConnection').value.length == 0){
        ok = false;
    }
    
    if(ok && validateEmail($id('emailConnection'))){
         $id('submitConnection').disabled = false;  
    } else{
         $id('submitConnection').disabled = true; 
    }
}

/**
   Fonctions inscriptions
**/

function checkInscription(){
    ok = true;
    if($id('pseudoInscription').value.length == 0){
       ok = false;
    } else if($id('emailInscription').value.length == 0){
        ok = false;
    }else if($id('passwordInscription').value.length == 0){
         ok = false;
    } else if($id('confirmPasswordInscription').value.length == 0){
         ok = false;
    }
    
    if(ok){
        if(checkPassword($id('passwordInscription'),$id('confirmPasswordInscription')) && validateEmail($id('emailInscription'))){
            $id('submitInscription').disabled = false; 
        }
    } else{
        $id('submitInscription').disabled = true; 
    }
}

function checkPasswordInscription(){
    checkPassword($id('passwordInscription'),$id('confirmPasswordInscription'), true);
}


/**
    Les fonctions génériques
**/

function $id(elem) {
	var type=typeof(elem);
	if (type=="string") {
		return document.getElementById(elem);
	}
	return elem;
}


function hide(elem) {
	var el=$id(elem);

	if (!el.defdisplay) {
	    if ((el.style.display != 'none') && (el.style.display != 'block')) {
		el.defdisplay=el.style.display;
	    }
	}

	el.style.display='none';
	return el;
}

function show(elem) {
	var el=$id(elem);

	if ((el.style.display != '') && (el.style.display != 'none')) {
	    return el;
	}

	if (el.defdisplay) {
	    el.style.display=el.defdisplay;
	} else {
	el.style.display='block';
	}
	return el;
}

function checkPassword(password, confirmPassword, isColor = false){
    var ok = false;
    if(password.value.length >= MIN_MDP && (password.value == confirmPassword.value)){
        ok = true;
        if(isColor){
            inputValid(password);
            inputValid(confirmPassword);
        }
    }
    else{
        if(isColor){
            inputError(password);
            inputError(confirmPassword);
        }
    } 
    return ok;
}

function inputValid(control){
    control.style.backgroundColor = "#DEF0D7";
    control.style.border = "solid 1px #B2DBA1";
}

function inputError(control){
    control.style.backgroundColor = "#F5E6E6";
    control.style.border = "solid 1px #DCA7A7";
}

function inputNeutre(control){
    control.style.backgroundColor = "#FFFFFF";
    control.style.border = "solid 1px #CCCCCC";
}

function validateEmail(mail, isColor = false)   
{ 
    var ok = false;
    if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail.value))  
    {  
        ok = true;
        if(isColor){
            inputValid(mail);
        }
    }else{
        inputError(mail);
    } 
    return ok;
}

function validatePageWithExtension(page){
    return (nomFichier() == page + '.php' || nomFichier() == page +'.html');
}

function nomFichier (){
    var CheminComplet = document.location.href;
    return  NomDuFichier = CheminComplet.substring(CheminComplet.lastIndexOf( "/" )+1 );
}

function redirect_post(link, post_var) {
    var form = '';
    $.each(post_var, function(key, value) {
        form+='<input type="hidden" name="'+key+'" value="'+value+'">';
    });
    $('<form class="hidden" action="'+link+'" method="POST">'+form+'</form>').appendTo('body').submit();
}