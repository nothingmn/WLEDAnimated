Add-Type -AssemblyName System.Net.Http

# Define parameters
$filePath = "C:\data\source\garage\WLEDAnimated\WLEDAnimated\WLEDAnimateConsole\ghosty.gif"
$uri = "http://localhost:5000/api/UploadImage"
$ipAddress = "10.0.0.217"
$port = 21324
$width = 32
$height = 8
$wait = 1
$pauseBetweenFrames = 100
$iterations = 1
# Create a multi-part form content
$form = New-Object System.Net.Http.MultipartFormDataContent
# Add file content
# Read file into byte array
$fileBytes = [System.IO.File]::ReadAllBytes($filePath)
# Create ByteArrayContent
$fileContent = New-Object System.Net.Http.ByteArrayContent($fileBytes)
$fileContent.Headers.ContentType = New-Object System.Net.Http.Headers.MediaTypeHeaderValue "application/octet-stream"
$form.Add($fileContent, "file", [System.IO.Path]::GetFileName($filePath))
# Add other parameters
$form.Add((New-Object System.Net.Http.StringContent($ipAddress)), "ipAddress")
$form.Add((New-Object System.Net.Http.StringContent($port)), "port")
$form.Add((New-Object System.Net.Http.StringContent($width)), "width")
$form.Add((New-Object System.Net.Http.StringContent($height)), "height")
$form.Add((New-Object System.Net.Http.StringContent($wait)), "wait")
$form.Add((New-Object System.Net.Http.StringContent($pauseBetweenFrames)), "pauseBetweenFrames")
$form.Add((New-Object System.Net.Http.StringContent($iterations)), "iterations")
# Create HttpClient
$client = New-Object System.Net.Http.HttpClient
# Send POST request
$response = $client.PostAsync($uri, $form).Result
# Output response
Write-Output $response
