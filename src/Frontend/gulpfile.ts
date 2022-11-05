/// <binding BeforeBuild='build' Clean='clean' />
const { src, dest, series, parallel } = require("gulp");
const cleancss = require("gulp-clean-css");
const del = require("del");
const filelog = require("gulp-filelog");
const gulpif = require("gulp-if");
const sass = require("gulp-dart-sass");
const sourcemaps = require("gulp-sourcemaps");
const ts = require("gulp-typescript");
const uglify = require("gulp-uglify");

let isProduction: boolean = process.env.NODE_ENV === "production";

let scssSettings =
{
    includePaths: ["node_modules/"],
    outputStyle: "expanded"
};

// Compiles all SCSS files
// except App.razor.scss
function compileComponentSCSS(): any
{
    return src(["**/*.scss", "!node_modules/**", "!wwwroot/**", "!./app.scss"])
        .pipe(filelog())
        .pipe(gulpif(!isProduction, sourcemaps.init()))
        .pipe(sass(scssSettings).on("error", sass.logError))
        .pipe(gulpif(isProduction, cleancss({ level: 2 })))
        .pipe(gulpif(!isProduction, sourcemaps.write("./")))
        .pipe(dest("./"));
}

// Compiles App.razor.scss
function compileAppSCSS(): any
{
    return src(["./app.scss"])
        .pipe(filelog())
        .pipe(gulpif(!isProduction, sourcemaps.init()))
        .pipe(sass(scssSettings).on("error", sass.logError))
        .pipe(gulpif(isProduction, cleancss({ level: 2 })))
        .pipe(gulpif(!isProduction, sourcemaps.write("./")))
        .pipe(dest("./wwwroot/css"));
}

// Compiles all Typescript files
function compileTypescript(): any
{
    // Create Typescript project
    let tsProject = ts.createProject("tsconfig.json");
    // Create pipe for compilation
    let tsResult = src(["**/*.ts", "!node_modules/**", "!gulpfile.ts", "!wwwroot/**"])
        .pipe(filelog())
        .pipe(gulpif(!isProduction, sourcemaps.init()))
        .pipe(tsProject());

    return tsResult.js.pipe(gulpif(!isProduction, sourcemaps.write("./")))
        .pipe(gulpif(isProduction, uglify()))
        .pipe(dest("./"));
}

// Copies any dependency files into the project
function restoreDependencies(): any
{
    return src("node_modules/open-iconic/font/fonts/*.*")
        .pipe(dest("wwwroot/css/open-iconic/font/fonts"));
}

// Removes any dependency files from the project
function cleanDependencies(): any
{
    return del(["wwwroot/css/open-iconic/font/fonts"]);
}

// Removes all compiled CSS files
function cleanSCSS(): any
{
    return del(["**/*.css", "**/*.css.map", "!node_modules/**"])
}

// Removes all compiled JavaScript files
function cleanTypescript(): any
{
    return del(["**/*.js", "**/*.js.map", "!node_modules/**", "!gulpfile.js", "!gulpfile.ts"]);
}

exports.compileSCSS = series(compileAppSCSS, compileComponentSCSS);
exports.compileTypescript = compileTypescript;
exports.restoreDependencies = restoreDependencies;
exports.clean = parallel(cleanDependencies, cleanSCSS, cleanTypescript);
exports.build =
    series(
        exports.clean,
        parallel(
            exports.compileSCSS,
            exports.restoreDependencies,
            exports.compileTypescript
        )
    );
