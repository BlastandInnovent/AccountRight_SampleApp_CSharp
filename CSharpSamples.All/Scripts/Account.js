//  File:        Account.cshtml
//  Copyright:   Copyright 2012 MYOB Technology Pty Ltd. All rights reserved.
//  Website:     http://www.myob.com
//  Author:      MYOB
//  E-mail:      info@myob.com
//
//Documentation, code and sample applications provided by MYOB Australia are for 
//information purposes only. MYOB Technology Pty Ltd and its suppliers make no 
//warranties, either express or implied, in this document. 
//
//Information in this document or code, including website references, is subject
//to change without notice. Unless otherwise noted, the example companies, 
//organisations, products, domain names, email addresses, people, places, and 
//events are fictitious. 
//
//The entire risk of the use of this documentation or code remains with the user. 
//Complying with all applicable copyright laws is the responsibility of the user. 
//
//Copyright 2012 MYOB Technology Pty Ltd. All rights reserved.
function deleteAccount(id, description, deleteAction, redirect) {
    confirmationMessageBox("Delete", "Delete account '" + description + "'. Are you sure?", confirmDeleteAccountResult,
        { journalId: id, redirect: redirect, deleteAction: deleteAction });
}

function confirmDeleteAccountResult(dialogResult, param) {
    if (dialogResult == true) {
        $.ajax({
            type: "POST",
            url: param.deleteAction,
            data: { id: param.journalId }
        }).done(function (data) {
            if (data.ok == false) {
                showMessageBox("Account", data.message);
            }
            else {
                window.location = param.redirect;
            };
        });
    }
}