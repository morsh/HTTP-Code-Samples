# azure-cs-json-translation
Translating the values in a JSON file (deeply) using Azure Cognitive Services

# Installation

```
npm install
```

# Execution

Where:

* AZURE_CS_API_KEY is the Azure Cognitive Services API Key
* ./fixtures/en.json is the JSON file to scan
* he,fr is an array of languages to translate into

```
node index.js AZURE_CS_API_KEY ./fixtures/en.json he,fr
```

# Credits
Code inspiration gotten from:
https://github.com/tewen/gtd-scripts