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
     * @param string $fromLanguage   Source language Code.
     * @param string $toLanguage     Target language Code.
     * @param string $category       Category.
     * @param string $contentType    Content Type.
     * @param string $user           User Type.
     * @param string $translationArr Translation Array.
     * 
     * @return string.
     */
    function createReqXML($fromLanguage,$toLanguage,$category,$contentType,$user,$translationArr) {
        //Create the XML string for passing the values.
        $requestXml = "<AddtranslationsRequest>";
        $requestXml .= "<AppId></AppId>";
        $requestXml  .=    "<From>$fromLanguage</From>";
        $requestXml .= "<Options>";
        $requestXml .= "<Category xmlns='http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2'>$category</Category>";
        $requestXml .= "<ContentType xmlns='http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2'>$contentType</ContentType>";
        $requestXml .= "<User xmlns='http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2'>$user</User>";
        $requestXml .= "<Uri xmlns='http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2'></Uri>";
        $requestXml .= "</Options>";
        $requestXml .= "<To>$toLanguage</To>";

        $requestXml .= "<Translations>";

        foreach($translationArr as $value) {
            $requestXml .= "<Translation xmlns='http://schemas.datacontract.org/2004/07/Microsoft.MT.Web.Service.V2'>";
            $requestXml .= "<OriginalText>".$value['OriginalText']."</OriginalText>";
            $requestXml .= "<Rating>".$value['rating']."</Rating>";
            $requestXml .= "<TranslatedText>".$value['TranslatedText']."</TranslatedText>";
            $requestXml .= "<Sequence>0</Sequence>";
            $requestXml .="</Translation>";
        }
        $requestXml .= "</Translations>";
        $requestXml .= "</AddtranslationsRequest>";
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

    //Set the Params.
    $fromLanguage    = "es";
    $toLanguage        = "en";
    $user            = 'guest';
    $category         = "general";
    $contentType    = "text/plain";

    //Create translation Array.
    $translationArr   = array();
    $translationArr[] = array(
        'OriginalText'   => 'una importante contribución a la rentabilidad de la empresa',
        'TranslatedText' => 'a significant contribution tothe company profitability',
         'Rating'         => 4 
    );

    $translationArr[] = array (
        'OriginalText'   => 'a veces los errores son divertidos',
        'TranslatedText' => 'in some cases errors are fun',
         'Rating'         => 4 
    );

    //Create the Translator Object.
    $translatorObj = new HTTPTranslator();
    
    //Get the Request XML.
    $requestXml = $translatorObj->createReqXML($fromLanguage,$toLanguage,$category,$contentType,$user,$translationArr);

    //HTTP AddTranslationArray URL.
    $url = "http://api.microsofttranslator.com/v2/Http.svc/AddTranslationArray";

    //Get the curl response.
    $curlResponse = $translatorObj->curlRequest($url, $authHeader, $requestXml);

    foreach ($translationArr as $translation)
    echo "Translation for <b>'".$translation["OriginalText"]."'</b> added successfully.<br/>";

} catch (Exception $e) {
    echo "Exception: " . $e->getMessage() . PHP_EOL;
}
