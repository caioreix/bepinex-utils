restore-release:
	dotnet restore /p:Configuration=Release

restore-test:
	dotnet restore /p:Configuration=Test

build-test: restore-test
	dotnet build . --configuration Test --no-restore

build-release: restore-release
	dotnet build . --configuration Release --no-restore
