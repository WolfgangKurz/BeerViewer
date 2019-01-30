function Main {
    # Remove and create dist directory
    Remove-Item ./dist -Recurse -Force | Out-Null
    New-Item ./dist -ItemType directory | Out-Null

    # Copy from source and remove *.ts and useless files
    Copy-Item ./source/* ./dist -Force -Recurse
    Remove-Item ./dist/* -Include *.ts -Recurse

    # Remove empty directories
    $tdc = "./dist"
    do {
        $dirs = gci $tdc -directory -recurse | Where { (gci $_.fullName).count -eq 0 } | select -expandproperty FullName
        $dirs | Foreach-Object { Remove-Item $_ }
    } while ($dirs.count -gt 0)

    # webpack compile
	"Compiling WindowFrame..."
    npx webpack --progress
	"Compiling Modules..."
    npx webpack --progress --config webpack.modules.config.js

	"Done."
}
Main