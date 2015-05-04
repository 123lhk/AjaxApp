/// <vs BeforeBuild='bower:dev' />
module.exports = function (grunt) {

	// Project configuration.
	grunt.initConfig({
		pkg: grunt.file.readJSON('package.json'),
		
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
					'angular-ui-grid': 'angular-ui-grid',
					'jquery': 'jquery/dist',
					'angular-loading-bar/loading-bar.min.js': 'angular-loading-bar/build/loading-bar.min.js',
					'angular-loading-bar/loading-bar.js': 'angular-loading-bar/build/loading-bar.js',
					'angular-local-storage/angular-local-storage.js': 'angular-local-storage/dist/angular-local-storage.js',
					'angular-local-storage/angular-local-storage.min.js': 'angular-local-storage/dist/angular-local-storage.min.js',
					'angular-filter/angular-filter.js': 'angular-filter/dist/angular-filter.js',
					'angular-filter/angular-filter.min.js': 'angular-filter/dist/angular-filter.min.js'
				}
			},
			css: {
				options: {
					destPrefix: 'css'
				},
				files: {
					'bootswatch': 'bootswatch/flatly',
					'angular-loading-bar/loading-bar.min.css': 'angular-loading-bar/build/loading-bar.min.css',
					'angular-loading-bar/loading-bar.css': 'angular-loading-bar/build/loading-bar.css',
					'animate.css/animate.css': 'animate.css/animate.css',
					'animate.css/animate.min.css': 'animate.css/animate.min.css',
					'font-awesome/.': 'font-awesome/css',
					'bootstrap-social/bootstrap-scocial.css': 'bootstrap-social/bootstrap-social.css'
				}
			},
			fonts: {
				options: {
					destPrefix: 'css/fonts'
				},
				files: {
					'.': 'font-awesome/fonts'
				}
			},
			fonts2: {
				options: {
					destPrefix: 'css/fonts'
				},
				files: {
					'.': 'bootstrap/dist/fonts',
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
	grunt.loadNpmTasks('grunt-contrib-cssmin');
	// Default task(s).
	//grunt.registerTask('default', ['uglify']);

};