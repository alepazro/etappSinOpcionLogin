# [Bootstrap v3.0.0](https://getbootstrap.com) [![Build Status](https://secure.travis-ci.org/twbs/bootstrap.png)](https://travis-ci.org/twbs/bootstrap)

Bootstrap is a sleek, intuitive, and powerful front-end framework for faster and easier web development, created and maintained by [Mark Otto](https://twitter.com/mdo) and [Jacob Thornton](https://twitter.com/fat).

To get started, check out [https://getbootstrap.com](https://getbootstrap.com)!



## Quick start

Three quick start options are available:

* [Download the latest release](https://github.com/twbs/bootstrap/zipball/3.0.0-wip).
* Clone the repo: `git clone git://github.com/twbs/bootstrap.git`.
* Install with [Bower](https://bower.io): `bower install bootstrap`.

Read the [Getting Started page](https://getbootstrap.com/getting-started/) for information on the framework contents, templates and examples, and more.



## Bugs and feature requests

Have a bug or a feature request? [Please open a new issue](https://github.com/twbs/bootstrap/issues). Before opening any issue, please search for existing issues and read the [Issue Guidelines](https://github.com/necolas/issue-guidelines), written by [Nicolas Gallagher](https://github.com/necolas/).

You may use [this JS Bin](https://jsbin.com/aKiCIDO/1/edit) as a template for your bug reports.



## Documentation

Bootstrap's documentation, included in this repo in the root directory, is built with [Jekyll](https://jekyllrb.com) and publicly hosted on GitHub Pages at [https://getbootstrap.com](https://getbootstrap.com). The docs may also be run locally.

### Running documentation locally

1. If necessary, [install Jekyll](https://jekyllrb.com/docs/installation) (requires v1.x).
2. From the root `/bootstrap` directory, run `jekyll serve` in the command line.
  - **Windows users:** run `chcp 65001` first to change the command prompt's character encoding ([code page](https://en.wikipedia.org/wiki/Windows_code_page)) to UTF-8 so Jekyll runs without errors.
3. Open [https://localhost:9001](https://localhost:9001) in your browser, and voilà.

Learn more about using Jekyll by reading their [documentation](https://jekyllrb.com/docs/home/).

### Documentation for previous releases

Documentation for v2.3.2 has been made available for the time being at [https://getbootstrap.com/2.3.2/](https://getbootstrap.com/2.3.2/) while folks transition to Bootstrap 3.

[Previous releases](https://github.com/twbs/bootstrap/releases) and their documentation are also available for download.



## Compiling CSS and JavaScript

Bootstrap uses [Grunt](https://gruntjs.com/) with convenient methods for working with the framework. It's how we compile our code, run tests, and more. To use it, install the required dependencies as directed and then run some Grunt commands.

### Install Grunt

From the command line:

1. Install `grunt-cli` globally with `npm install -g grunt-cli`.
2. Install the [necessary local dependencies](package.json) via `npm install`

When completed, you'll be able to run the various Grunt commands provided from the command line.

**Unfamiliar with `npm`? Don't have node installed?** That's a-okay. npm stands for [node packaged modules](https://npmjs.org/) and is a way to manage development dependencies through node.js. [Download and install node.js](https://nodejs.org/download/) before proceeding.

### Available Grunt commands

#### Build - `grunt`
Run `grunt` to run tests locally and compile the CSS and JavaScript into `/dist`. **Requires [recess](https://github.com/twitter/recess) and [uglify-js](https://github.com/mishoo/UglifyJS).**

#### Only compile CSS and JavaScript - `grunt dist`
`grunt dist` creates the `/dist` directory with compiled files. **Requires [recess](https://github.com/twitter/recess) and [uglify-js](https://github.com/mishoo/UglifyJS).**

#### Tests - `grunt test`
Runs jshint and qunit tests headlessly in [phantomjs](https://github.com/ariya/phantomjs/) (used for CI). **Requires [phantomjs](https://github.com/ariya/phantomjs/).**

#### Watch - `grunt watch`
This is a convenience method for watching just Less files and automatically building them whenever you save.

### Troubleshooting dependencies

Should you encounter problems with installing dependencies or running Grunt commands, uninstall all previous dependency versions (global and local). Then, rerun `npm install`.



## Contributing

Please read through our guidelines for contributing to Bootstrap. Included are directions for opening issues, coding standards, and notes on development.

More over, if your pull request contains JavaScript patches or features, you must include relevant unit tests. All HTML and CSS should conform to the [Code Guide](https://github.com/mdo/code-guide), maintained by [Mark Otto](https://github.com/mdo).

Editor preferences are available in the [editor config](.editorconfig) for easy use in common text editors. Read more and download plugins at [https://editorconfig.org](https://editorconfig.org).



## Community

Keep track of development and community news.

* Follow [@twbootstrap on Twitter](https://twitter.com/twbootstrap).
* Read and subscribe to the [The Official Bootstrap Blog](https://blog.getbootstrap.com).
* Have a question that's not a feature request or bug report? [Ask on the mailing list.](https://groups.google.com/group/twitter-bootstrap)
* Chat with fellow Bootstrappers in IRC. On the `irc.freenode.net` server, in the `##twitter-bootstrap` channel.




## Versioning

For transparency and insight into our release cycle, and for striving to maintain backward compatibility, Bootstrap will be maintained under the Semantic Versioning guidelines as much as possible.

Releases will be numbered with the following format:

`<major>.<minor>.<patch>`

And constructed with the following guidelines:

* Breaking backward compatibility bumps the major (and resets the minor and patch)
* New additions without breaking backward compatibility bumps the minor (and resets the patch)
* Bug fixes and misc changes bumps the patch

For more information on SemVer, please visit [https://semver.org/](https://semver.org/).



## Authors

**Mark Otto**

+ [https://twitter.com/mdo](https://twitter.com/mdo)
+ [https://github.com/mdo](https://github.com/mdo)

**Jacob Thornton**

+ [https://twitter.com/fat](https://twitter.com/fat)
+ [https://github.com/fat](https://github.com/fat)



## Copyright and license

Copyright 2012 Twitter, Inc under [the Apache 2.0 license](LICENSE).
