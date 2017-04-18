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
    function createReqXML($inputStrArr) {
        $requestXml = '<ArrayOfstring xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">';
        if(sizeof($inputStrArr) > 0){
            foreach($inputStrArr as $str)
            $requestXml .= "<string>$str</string>";
        } else {
            throw new Exception('$inputStrArr array is empty.');
        }
        $requestXml .= '</ArrayOfstring>';
        return $requestXml;
    }
}

try {
    //Client Secret key of the application.
    $clientSecret = "clientsecret";

    //Create the AccessTokenAuthentication object.
    $authObj      = new AccessTokenAuthentication();
    //Get the Access token.
    $accessToken  = $authObj->getToken($clientSecret);
    //Create the authorization Header string.
    $authHeader = "Authorization: Bearer ". $accessToken;

    //Input string array.
    $inputStrArr = array (
                    "les erreurs sont parfois amusants",
                    "you can try to enter a longer phrase", 
                    "Welche Sprache ist das?"
                    );

    //Create the Translator Object.
    $translatorObj = new HTTPTranslator();

    //Create the Request XML.
    $requestXml = $translatorObj->createReqXML($inputStrArr);

    // HTTP detectMehod URL
    $detectMethodUrl = "http://api.microsofttranslator.com/V2/Http.svc/DetectArray";

    // Call the curl Request.
    $curlResponse = $translatorObj->curlRequest($detectMethodUrl, $authHeader, $requestXml);

    // Interprets a string of XML into an object.
    $xmlObj = simplexml_load_string($curlResponse);
    $languageCodesArr = array();
    foreach($xmlObj->string as $language){
        $languageCodesArr[] = $language;
    }


    /*
     * Get the Full language name.
     *
     */

    //Language Code Array.
    $locale = 'en';

    //Create the XML string for passing the values.
    $requestXml = '<ArrayOfstring xmlns="http://schemas.microsoft.com/2003/10/Serialization/Arrays" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">';
    if(sizeof($languageCodesArr) > 0){
        foreach($languageCodesArr as $codes)
        $requestXml .= "<string>$codes</string>";
    } else {
        throw new Exception('$languageCodes array is empty.');
    }
    $requestXml .= '</ArrayOfstring>';

    //HTTP GetlanguageNames URL.
    $url = "http://api.microsofttranslator.com/V2/Http.svc/GetLanguageNames?locale=$locale";

    // Call the curlRequest.
    $curlResponse = $translatorObj->curlRequest($url, $authHeader, $requestXml);
    // Interprets a string of XML into an object.
    $xmlObj = simplexml_load_string($curlResponse);

    $i = 0;
    echo "<table border=2px>";
    echo "<tr>";
    echo "<td><b>LanguageCodes</b></td><td><b>Language Names</b></td>";
    echo "</tr>";
    foreach ($xmlObj->string as $language) {
        echo "<tr><td>".$inputStrArr[$i]."</td><td>". $languageCodesArr[$i]."(".$language.")"."</td></tr>";
        $i++;
    }
    echo "</table>";
} catch (Exception $e) {
    echo "Exception: " . $e->getMessage() . PHP_EOL;
}
