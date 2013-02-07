//  File:        OAuthInfo.cs
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
namespace CSharpSamples.Common
{
    public class OAuthInfo
    {
        public string AuthorizationUrl { get; set; }
        public string TokenUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string Scope { get; set; }
        public OAuthToken Token { get; set; }
        public string AccessToken { get { return Token == null ? string.Empty : Token.AccessToken; } }
        public string RefreshToken { get { return Token == null ? string.Empty : Token.RefreshToken; } }
        public string ExpiresIn { get { return Token == null ? string.Empty : Token.ExpiresIn.ToString() + "ms"; } }
        public string TokenType { get { return Token == null ? string.Empty : Token.TokenType; } }
    }
}
