# This script creates .zip files containing release versions for alla available runtimes and platforms in the .publish folder

$platforms = @(
    'Desktop'
	# 'Android',
	# 'Browser',
	# 'iOS'
)

$runtimes = @(
    'win-x86',
    'win-x64',
	'win-arm64'
	
    'linux-x64',
	'linux-musl-x64',
	'linux-musl-arm64',
	'linux-arm',
	'linux-arm64',
	'linux-bionic-arm64',
	
	#'osx-x64',
	#'osx-arm64',
	#'ios-arm64',
	
	#'android-arm64'
)

ForEach ($platform in $platforms) {
	ForEach ($runtime in $runtimes) {
		echo "Building $platform - $runtime..."
		$targetDir = ".publish/$runtime/$platform"
		#echo $targetDir
		if (Test-Path -LiteralPath $targetDir) {
			Remove-Item $targetDir -Recurse
		}
		dotnet publish "Fly.$platform\Fly.$platform.csproj" -c Release -o $targetDir --runtime $runtime --self-contained=true /p:platform="Any CPU" /p:configuration="Release"
		Compress-Archive -Path $targetDir -DestinationPath ".publish/$platform-$runtime.zip"
	}
}