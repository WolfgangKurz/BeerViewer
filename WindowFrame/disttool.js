var fs = require("fs");
var path = require("path");
var glob = require("glob");

const skip_clean = [".js", ".js.map", ".d.ts"];
const skip_copy = [".ts", ".scss"];

// Clean dist
glob.sync("./dist/**/*")
	.filter(x => !skip_clean.some(y => x.endsWith(y)))
	.filter(x => !fs.lstatSync(x).isDirectory())
	.forEach(x => fs.unlinkSync(x));

// Copy to dist
glob.sync("./source/**/*")
	.filter(x => !skip_copy.some(y => x.endsWith(y)))
	.filter(x => !fs.lstatSync(x).isDirectory())
	.forEach(x => {
		var y = x.replace("/source/", "/dist/");
		if (fs.existsSync(y)) fs.unlinkSync(y);

		var z = path.dirname(y);
		if (!fs.existsSync(z))
			fs.mkdirSync(z, { recursive: true });
		fs.copyFileSync(x, y);
	});

// Delete empty
while (true) {
	const list = glob.sync("./dist/**")
		.filter(x => x !== "./dist")
		.filter(x => fs.lstatSync(x).isDirectory())
		.filter(x => glob.sync(x + "/**").length <= 1);

	if (list.length === 0) break;

	list.forEach(x => fs.rmdirSync(x));
}
return true;