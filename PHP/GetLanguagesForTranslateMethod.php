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
     * @param string $postData   Data to post.
     *
     * @return string.
     *
     */
    function curlRequest($url, $authHeader, $postData=''){
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
        if($postData) {
            //Set HTTP POST Request.
            curl_setopt($ch, CURLOPT_POST, TRUE);
            //Set data to POST in HTTP "POST" Operation.
            curl_setopt($ch, CURLOPT_POSTFIELDS, $postData);
        }
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

    /*
     * Create Request XML Format.
     *
     * @param string $languageCode  Language code
     *
     * @return string.
     */
    function createReqXML($languageCodes) {
        //Create the Request XML.
        $requestXml = '<ArrayOfstring xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">';
        if(sizeof($languageCodes) > 0){
            foreach($languageCodes as $codes)
            $requestXml .= "<string>$codes</string>";
        } else {
            throw new Exception('$languageCodes array is empty.');
        }
        $requestXml .= '</ArrayOfstring>';
        return $requestXml;
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

    //Call the Detect Method.
    $url = "http://api.microsofttranslator.com/V2/Http.svc/GetLanguagesForTranslate";
    
    //Create the Translator Object.
    $translatorObj = new HTTPTranslator();
    
    //Call Curl Request.//
    $strResponse = $translatorObj->curlRequest($url, $authHeader);

    // Interprets a string of XML into an object.
    $xmlObj = simplexml_load_string($strResponse);

    $languageCodes = array();
    foreach($xmlObj->string as $language){
        $languageCodes[] = $language;
    }

    /*
     * Get the language Names from languageCodes.
     */
    $locale = 'en';
    $getLanguageNamesurl = "http://api.microsofttranslator.com/V2/Http.svc/GetLanguageNames?locale=$locale";

    //Get the Request XML Format.
    $requestXml = $translatorObj->createReqXML($languageCodes);

    //Call the curlReqest.
    $curlResponse = $translatorObj->curlRequest($getLanguageNamesurl, $authHeader, $requestXml);
    
    //Parse the XML string.
    $xmlObj = simplexml_load_string($curlResponse);
    $i=0;
    echo "<table border=2px>";
    echo "<tr>";
    echo "<td><b>LanguageCodes</b></td><td><b>Language Names</b></td>";
    echo "</tr>";
    foreach ($xmlObj->string as $language) {
        echo "<tr><td>".$languageCodes[$i]."</td><td>". $language ."</td></tr>";
        $i++;
    }
    echo "</table>";
} catch (Exception $e) {
    echo "Exception: " . $e->getMessage() . PHP_EOL;
}
