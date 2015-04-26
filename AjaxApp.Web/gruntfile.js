/// <vs BeforeBuild='bower:dev' />
module.exports = function (grunt) {

	// Project configuration.
	grunt.initConfig({
		pkg: grunt.file.readJSON('package.json'),
		uglify: {
			options: {
				banner: '/*! <%= pkg.name %> <%= grunt.template.today("yyyy-mm-dd") %> */\n'
			},
			build: {
				src: 'src/<%= pkg.name %>.js',
				dest: 'build/<%= pkg.name %>.min.js'
			}
		},
		bower: {
			install: {
			}
		},
		bowercopy: {
			options: {
				runBower: true,
				srcPrefix: 'components'
			},
			js: {
				options: {
					destPrefix: 'js'
				},
				files: {
					'angular': 'angular',
					'angular-route': 'angular-route',
					'angular-animate': 'angular-animate',
					'angular-bootstrap': 'angular-bootstrap',
					'jquery': 'jquery/dist',
					'angular-loading-bar/loading-bar.min.js': 'angular-loading-bar/build/loading-bar.min.js',
					'angular-loading-bar/loading-bar.js': 'angular-loading-bar/build/loading-bar.js',
					'angular-local-storage/angular-local-storage.js': 'angular-local-storage/dist/angular-local-storage.js',
					'angular-local-storage/angular-local-storage.min.js': 'angular-local-storage/dist/angular-local-storage.min.js'
				}
			},
			css: {
				options: {
					destPrefix: 'css'
				},
				files: {
					'bootswatch': 'bootswatch/flatly',
					'angular-loading-bar/loading-bar.min.css': 'angular-loading-bar/build/loading-bar.min.css',
					'angular-loading-bar/loading-bar.css': 'angular-loading-bar/build/loading-bar.css'
				}
			},
			fonts: {
				options: {
					destPrefix: 'fonts'
				},
				files: {
					'bootstrap': 'bootstrap/dist/fonts'
					
				}
			}
		}

	});

	// Load the plugin that provides the "uglify" task.
	grunt.loadNpmTasks('grunt-contrib-uglify');
	grunt.loadNpmTasks('grunt-bower');
	grunt.loadNpmTasks('grunt-bower-task');
	grunt.loadNpmTasks('grunt-bowercopy');
	grunt.loadNpmTasks('grunt-contrib-copy');
	grunt.loadNpmTasks('grunt-contrib-concat');
	grunt.loadNpmTasks('grunt-contrib-clean');
	// Default task(s).
	//grunt.registerTask('default', ['uglify']);

};