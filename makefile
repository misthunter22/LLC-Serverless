.PHONY: build clean start stop list package

clean:
	rm -force .\bower_components
	rm -force .\node_modules
	rm -force .\public
	rm -force .\dist

build: clean
	npm install
	./node_modules/bower/bin/bower install
	./node_modules/brunch/bin/brunch build

production: clean
	npm install
	./node_modules/bower/bin/bower install --production
	./node_modules/brunch/bin/brunch build --production

start:
	./node_modules/brunch/bin/brunch watch --server

stop:
	./node_modules/pm2/bin/pm2 stop pm2-config.json
	./node_modules/pm2/bin/pm2 delete pm2-config.json
	./node_modules/pm2/bin/pm2 kill

list:
	./node_modules/pm2/bin/pm2 list

package:
	mkdir dist
	tar -czvf dist/public.tar.gz public