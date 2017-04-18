<?php

class AccessTokenAuthentication {
    /*
     * Get the access token.
     *
     * @param string $azure_key    Subscription key for Text Translation API.
     *
     * @return string.
     */
    function getToken($azure_key)
    {
        $url = 'https://api.cognitive.microsoft.com/sts/v1.0/issueToken';
        $ch = curl_init();
        $data_string = json_encode('{body}');
        curl_setopt($ch, CURLOPT_POSTFIELDS, $data_string);
        curl_setopt($ch, CURLOPT_HTTPHEADER, array(
                'Content-Type: application/json',
                'Content-Length: ' . strlen($data_string),
                'Ocp-Apim-Subscription-Key: ' . $azure_key
            )
        );
        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_HEADER, false);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, TRUE);
        $strResponse = curl_exec($ch);
        curl_close($ch);
        return $strResponse;
    }
}

/*
 * Class:HTTPTranslator
 *
 * Processing the translator request.
 */
Class HTTPTranslator {
    /*
     * Create and execute the HTTP CURL request.
     *
     * @param string $url        HTTP Url.
     * @param string $authHeader Authorization Header string.
     *
     * @return string.
     *
     */
    function curlRequest($url, $authHeader){
        //Initialize the Curl Session.
        $ch = curl_init();
        //Set the Curl url.
        curl_setopt ($ch, CURLOPT_URL, $url);
        //Set the HTTP HEADER Fields.
        curl_setopt ($ch, CURLOPT_HTTPHEADER, array($authHeader,"Content-Type: text/xml"));
        //CURLOPT_RETURNTRANSFER- TRUE to return the transfer as a string of the return value of curl_exec().
        curl_setopt ($ch, CURLOPT_RETURNTRANSFER, TRUE);
        //CURLOPT_SSL_VERIFYPEER- Set FALSE to stop cURL from verifying the peer's certificate.
        curl_setopt ($ch, CURLOPT_SSL_VERIFYPEER, False);
        //Execute the  cURL session.
        $curlResponse = curl_exec($ch);
        //Get the Error Code returned by Curl.
        $curlErrno = curl_errno($ch);
        if ($curlErrno) {
            $curlError = curl_error($ch);
            throw new Exception($curlError);
        }
        //Close a cURL session.
        curl_close($ch);
        return $curlResponse;
    }
}

try {
    //Client Secret key of the application.
    $clientSecret = "ClientSecret";

    //Create the AccessTokenAuthentication object.
    $authObj      = new AccessTokenAuthentication();
    //Get the Access token.
    $accessToken  = $authObj->getToken($clientSecret);
    //Create the authorization Header string.
    $authHeader = "Authorization: Bearer ". $accessToken;

    //Set the Params.
    $originalText    = "una importante contribución a la rentabilidad de la empresa";
    $translatedText = "an important contribution to the company profitability";
    $fromLanguage    = "es";
    $toLanguage        = "en";
    $user            = 'TestUser';
    $category         = "general";
    $uri             = null;
    $rating         = 3;
    $contentType    ="text/plain";

    //Create the string for passing the values through GET method.
    $params = "originaltext=".urlencode($originalText).
              "&translatedtext=".urlencode($translatedText).
              "&from=".$fromLanguage.
              "&to=".$toLanguage.
              "&user=".$user.
              "&uri=".$uri.
              "&rating=".$rating.
              "&contentType=".$contentType.
              "&categsory=".$category;    

    //HTTP AddTranslation URL.
    $addTranslationArr = "http://api.microsofttranslator.com/V2/Http.svc/AddTranslation?$params";
    
    //Create the Translator Object.
    $translatorObj = new HTTPTranslator();
    
    //Call the HTTP curl request.
    $translatorObj->curlRequest($addTranslationArr, $authHeader);
    echo "Translation for <b>'$originalText'</b> added successfully.".PHP_EOL;

} catch (Exception $e) {
    echo "Exception: " . $e->getMessage() . PHP_EOL;
}
