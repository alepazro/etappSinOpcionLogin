﻿module.exports = function (grunt) {
    // * Read command-line switches
    // - Read in --browsers CLI option; split it on commas into an array if it's a string, otherwise ignore it
    var browsers = typeof grunt.option('browsers') == 'string' ? grunt.option('browsers').split(',') : undefined;

    var stripBanner = function (src, options) {

        if (!options) { options = {}; }
        var m = [];
        if (options.line) {
            // Strip // ... leading banners.
            m.push('(/{2,}[\\s\\S].*)');
        }
        if (options.block) {
            // Strips all /* ... */ block comment banners.
            m.push('(\/+\\*+[\\s\\S]*?\\*\\/+)');
        } else {
            // Strips only /* ... */ block comment banners, excluding /*! ... */.
            m.push('(\/+\\*+[^!][\\s\\S]*?\\*\\/+)');

        }
        var re = new RegExp('\s*(' + m.join('|') + ')\s*', 'g');
        src = src.replace(re, '');
        src = src.replace(/\s{2,}(\r|\n|\s){2,}$/gm, '');
        return src;
    };
    
    grunt.registerMultiTask('concat', 'Concatenate files.', function () {
        // Merge task-specific and/or target-specific options with these defaults.
        var options = this.options({
            separator: grunt.util.linefeed,
            banner: '',
            footer: '',
            stripBanners: false,
            process: false
        });
        // Normalize boolean options that accept options objects.
        if (typeof options.stripBanners === 'boolean' && options.stripBanners === true) { options.stripBanners = {}; }
        if (typeof options.process === 'boolean' && options.process === true) { options.process = {}; }

        // Process banner and footer.
        var banner = grunt.template.process(options.banner);
        var footer = grunt.template.process(options.footer);

        // Iterate over all src-dest file pairs.
        this.files.forEach(function (f) {
            // Concat banner + specified files + footer.
            var src = banner + f.src.filter(function (filepath) {
                // Warn on and remove invalid source files (if nonull was set).
                if (!grunt.file.exists(filepath)) {
                    grunt.log.warn('Source file "' + filepath + '" not found.');
                    return false;
                } else {
                    return true;
                }
            }).map(function (filepath) {
                // Read file source.
                var src = grunt.file.read(filepath);
                // Process files as templates if requested.
                if (options.process) {
                    src = grunt.template.process(src, options.process);
                }
                // Strip banners if requested.
                if (options.stripBanners) {
                    src = stripBanner(src, options.stripBanners);
                }
                return src;
            }).join(grunt.util.normalizelf(options.separator)) + footer;

            // Write the destination file.
            grunt.file.write(f.dest, src);

            // Print a success message.
            grunt.log.writeln('File "' + f.dest + '" created.');
        });
    });

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        templates: [
            'src/templates/**.html'
        ],
        srcFiles: [
            'src/*.js',
            'src/filters/*.js',
            'src/services/*.js',
            'src/classes/*.js',
            
            'src/directives/*.js',
            'src/i18n/*.js',
            '<%= ngtemplates.ngGrid.dest %>'
        ],
        testFiles: { //unit & e2e goes here
            karmaUnit: 'config/karma.conf.js',
            //unit: ['test/unit/*.js']
        },
        karma: {
            unit: {
                options: {
                    configFile: '<%= testFiles.karmaUnit %>',
                    autoWatch: false,
                    singleRun: true,
                    browsers: browsers || ['Chrome']
                },
            },
            watch: {
                options: {
                    configFile: '<%= testFiles.karmaUnit %>',
                    autoWatch: false,
                    browsers: browsers || ['Chrome']
                },
                background: true
            },
            e2e: {
                configFile: 'config/karma-e2e.conf.js',
                autoWatch: false,
                singleRun: true,
                browsers: browsers || ['Chrome']
            },
            midway: {
                configFile: 'config/karma-midway.conf.js',
                autoWatch: false,
                singleRun: true,
                browsers: browsers || ['Chrome']
            },
            ci: {
                options: {
                    configFile: '<%= testFiles.karmaUnit %>',
                    autoWatch: false,
                    singleRun: true,
                    browsers: browsers || ['PhantomJS']
                },
            }
        },
        watch: {
            // Run unit test with karma
            karma: {
                files: ['build/ng-grid.debug.js', 'test/unit/**/*.js', 'plugins/*.js'],
                tasks: ['karma:watch:run']
            },
            // Auto-build ng-grid.debug.js when source files change
            debug: {
                files: ['<%= srcFiles %>'],
                tasks: ['debug']
            },
            less: {
                files: ['src/less/**/*.less'],
                tasks: ['less']
            }
        },
        ngtemplates: {
            ngGrid: {
                options: { base: 'src/templates' },
                src: ['src/templates/**.html'],
                dest: 'build/templates.js'
            }
        },
        concat: {
            options: {
                banner: '/***********************************************\n' +
                    '* ng-grid JavaScript Library\n' +
                    '* Authors: https://github.com/angular-ui/ng-grid/blob/master/README.md \n' +
                    '* License: MIT (https://www.opensource.org/licenses/mit-license.php)\n' +
                    '* Compiled At: <%= grunt.template.today("mm/dd/yyyy HH:MM") %>\n' +
                    '***********************************************/\n' +
                    '(function(window, $) {\n' +
                    '\'use strict\';\n',
                footer: '\n}(window, jQuery));'
            },
            prod: {
                options: {
                    stripBanners: {
                        block: true,
                        line: true
                    }
                },
                src: ['<%= srcFiles %>'],
                dest: 'build/<%= pkg.name %>.js'
            },
            debug: {
                src: ['<%= srcFiles %>'],
                dest: 'build/<%= pkg.name %>.debug.js'
            },
            version: {
                src: ['<%= srcFiles %>'],
                dest: '<%= pkg.name %>-<%= pkg.version %>.debug.js'
            }
        },
        uglify: {
            build: {
                src: 'build/<%= pkg.name %>.js',
                dest: 'build/<%= pkg.name %>.min.js'
            },
            version: {
                src: '<%= pkg.name %>-<%= pkg.version %>.debug.js',
                dest: '<%= pkg.name %>-<%= pkg.version %>.min.js'
            }
        },
        clean: {
            templates: {
                src: ["<%= ngtemplates.ngGrid.dest %>"]
            }
        },
        less: {
            build: {
                options: {
                    // yuicompress: true
                },
                files: {
                    "ng-grid.css": ["src/less/global.less"]
                }
            },
            prod: {
                options: {
                    yuicompress: true
                },
                files: {
                    "ng-grid.min.css": ["src/less/global.less"]
                }
            }
        },
        jshint: {
            options: {
                boss: true,
                browser: true,
                camelcase: true,
                curly: true,
                eqeqeq: true,
                eqnull: true,
                // forin: true,
                immed: true,
                // indent: 4,
                latedef: true,
                // newcap: true,
                noarg: true,
                sub: true,
                // undef: true,
                // unused: true,
                globals: {
                    angular: false,
                    $: false
                }
            },
            src: [ 'src/**/*.js', 'plugins/**/*.js' ],
            spec: {
                options: {
                    camelcase: false,
                    globals: {
                        $: false,
                        angular: false,
                        beforeEach: false,
                        browser: false,
                        browserTrigger: false,
                        describe: false,
                        expect: false,
                        inject: false,
                        input: false,
                        it: false,
                        module: false,
                        repeater: false,
                        runs: false,
                        spyOn: false,
                        waits: false,
                        waitsFor: false,

                        ngGridWYSIWYGPlugin: false,
                        ngMidwayTester: false
                    }
                },
                files: {
                    spec: ['test/**/*.js', '!test/lib/**/*.js']
                },
            }
        }
    });

    // Load grunt-karma task plugin
    grunt.loadNpmTasks('grunt-karma');
    // Load the grunt-contrib-watch plugin for doing file watches
    grunt.loadNpmTasks('grunt-contrib-watch');

    grunt.loadNpmTasks('grunt-contrib-jshint');

    grunt.registerTask('test', ['jshint', 'karma:unit']);

    // Task for development; auto-build ng-grid.debug.js on source file changes, auto-test on ng-grid.debug.js or unit test changes
    grunt.registerTask('testwatch', ['jshint', 'karma:watch', 'watch']);

    grunt.registerTask('test-ci', ['jshint', 'debug', 'karma:ci']);

    // Load the plugin that provides the "uglify" task.
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-less');
    //grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-jsdoc');
    grunt.loadNpmTasks('grunt-angular-templates');
    grunt.loadNpmTasks('grunt-contrib-clean');

    // Old default task
    grunt.registerTask('build', ['less', 'ngtemplates', 'concat', 'uglify', 'clean']);

    // Default task(s).
    grunt.registerTask('default', 'No default task', function() {
        grunt.log.write('The old default task has been moved to "build" to prevent accidental triggering');
    });

    grunt.registerTask('debug', ['less', 'ngtemplates', 'concat:debug', 'clean']);
    grunt.registerTask('prod', ['less', 'ngtemplates', 'concat:prod', 'uglify', 'clean']);
    grunt.registerTask('version', ['ngtemplates', 'concat:version', 'uglify:version', 'clean']);
};