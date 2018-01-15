import React            from 'react';
import { PacmanLoader } from 'react-spinners';
import { idToken }      from '../Auth/Auth';

const $ = require('jquery');

export const AwsConstants = {
  invokeUrl: 'https://j6nbqh9lm5.execute-api.us-west-2.amazonaws.com/Prod',
};

export default function servicesBase(Component) {

  class ServicesBase extends Component {
	
	spinnerMarkup() {
	  let spinner = 
	    <div id="loading_spinner" style={{'marginLeft': '50%'}}>
	      <PacmanLoader
	        color={'#0ce3ac'} 
		  />
	    </div>;
		
	  return spinner;
	}
	
	changeSpinner(that, state) {
	  that.setState({loading: state});
	  if (state)
		$('#loading_spinner').show();
	  else
		$('#loading_spinner').hide();
	}
		
	sources() {
	  return new Promise(function (fulfill, reject) {
	    var pathTemplate = AwsConstants.invokeUrl + '/api/sources';
		var method       = 'GET';
		  
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
            var push = [];
		  	for (var i = 0; i < result.length; i++) {
			  var s = result[i];
			  var obj  = {};
			  obj['source']       = s.id;
			  obj['title']        = s.name;
			  obj['description']  = s.description;
			  obj['s3name']       = s.s3bucketName;
			  obj['allowlink']    = s.allowLinkChecking;
			  obj['allowextract'] = s.allowLinkExtractions; 
			  obj['created']      = s.dateCreated;
				  
			  push.push(obj);
			}
			
			fulfill(push);
          })
		 .catch(function(result) {
		    console.log(result);
			reject(result);
         });
	  });
	}
	
	source(id) {	  
	  return new Promise(function (fulfill, reject) {
	    var pathTemplate = AwsConstants.invokeUrl + '/api/sources/' + id;
		var method       = 'GET';
		  
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			var s = result;
			var obj  = {};
			obj['source']       = s.id;
			obj['title']        = s.name;
			obj['description']  = s.description;
			obj['s3name']       = s.s3bucketName;
			obj['allowlink']    = s.allowLinkChecking;
			obj['allowextract'] = s.allowLinkExtractions; 
			obj['created']      = s.dateCreated;
			obj['access']       = s.accessKey;
			obj['secret']       = s.secretKey;
			obj['region']       = s.region;
			obj['prefix']       = s.s3bucketSearchPrefix;
			obj['bucket']       = s.s3bucketId;
			
			fulfill(obj);
          })
		 .catch(function(result) {
		    console.log(result);
			reject(result);
         });
	  });
	}
	
	changeSource(source) {
	  return new Promise(function (fulfill, reject) {
	    var pathTemplate = AwsConstants.invokeUrl + '/api/sources';
		var method       = 'POST';
		  
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method,
		  body: JSON.stringify(source)
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			fulfill(result);
          })
		 .catch(function(result) {
		    console.log(result);
			reject(result);
         });
	  });
	}
	
	settings() {	  
	  return new Promise(function (fulfill, reject) {
	    var pathTemplate = AwsConstants.invokeUrl + '/api/settings';
		var method       = 'GET';
		  
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			var push = [];
		  	for (var i = 0; i < result.length; i++) {
			  var s = result[i];
			  var obj  = {};
			  obj['id']          = s.id;
			  obj['created']     = s.dateCreated;
			  obj['modified']    = s.dateModified;
			  obj['description'] = s.description;
			  obj['user']        = s.modifiedUser;
			  obj['name']        = s.name;
			  obj['value']       = s.value;
				  
			  push.push(obj);
			}
			
			fulfill(push);
          })
		 .catch(function(result) {
		    console.log(result);
			reject(result);
         });
	  });
	}
	
	setting(id) {  
	  return new Promise(function (fulfill, reject) {
	    var pathTemplate = AwsConstants.invokeUrl + '/api/settings/' + id;
		var method       = 'GET';
		  
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			var s = result;
			var obj  = {};
			obj['id']          = s.id;
			obj['name']        = s.name;
			obj['value']       = s.value;
			obj['description'] = s.description;
			
			fulfill(obj);
          })
		 .catch(function(result) {
		    console.log(result);
			reject(result);
         });
	  });
	}
	
	changeSetting(setting) {
	  return new Promise(function (fulfill, reject) {
	    var pathTemplate = AwsConstants.invokeUrl + '/api/settings';
		var method       = 'POST';
		  
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method,
		  body: JSON.stringify(setting)
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			fulfill(result);
          })
		 .catch(function(result) {
		    console.log(result);
			reject(result);
         });
	  });
	}
	
	packages() {
	  return new Promise(function (fulfill, reject) {
	    var pathTemplate = AwsConstants.invokeUrl + '/api/uploads';
		var method       = 'GET';
		  
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
            var push = [];
		  	for (var i = 0; i < result.length; i++) {
			  var s = result[i];
			  var obj  = {};
			  obj['id']               = s.id;
			  obj['name']             = s.name;
			  obj['description']      = s.description;
			  obj['uploadedBy']       = s.uploadedBy;
			  obj['dateUploaded']     = s.dateUploaded;
			  obj['key']              = s.key; 
			  obj['packageProcessed'] = s.packageProcessed;
			  obj['fileName']         = s.fileName;
			  obj['imsSchema']        = s.imsSchema;
			  obj['imsSchemaVersion'] = s.imsSchemaVersion;
			  obj['imsTitle']         = s.imsTitle;
				  
			  push.push(obj);
			}
			
			fulfill(push);
          })
		 .catch(function(result) {
		    console.log(result);
			reject(result);
         });
	  });
	}
	
	createPackage(pckg) {
	  return new Promise(function (fulfill, reject) {
	    var pathTemplate = AwsConstants.invokeUrl + '/api/uploads';
		var method       = 'POST';
		
		var fd = new FormData();
		fd.append('name', pckg.Name);
		fd.append('fileName', pckg.FileName);
		fd.append('file', pckg.FileContents);
		fd.append('description', pckg.Description);

		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Authorization': idToken()
	      }),
		  method: method,
		  body: fd
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			fulfill(result);
          })
		 .catch(function(result) {
		    console.log(result);
			reject(result);
         });
	  });
	}
	
	invalidLinks(settings) {
	  return new Promise(function (fulfill, reject) {		
		var pathTemplate = AwsConstants.invokeUrl + '/api/invalidReport';
		var body         = {
	      draw:       settings.draw, 
		  start:      settings.start, 
		  length:     settings.length,
		  column:     settings.order[0].column,
		  columnName: settings.columns[settings.order[0].column].data,
		  direction:  settings.order[0].dir,
		  search:     settings.search.value
	    };
		  
		var method = 'POST';
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method,
		  body: JSON.stringify(body)
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			fulfill(result);
          })
		  .catch(function(result) {
		    console.log(result);
		    reject(result);
          });
	  });
	}
	
	warningLinks(settings) {
	  return new Promise(function (fulfill, reject) {		
		var pathTemplate = AwsConstants.invokeUrl + '/api/warningReport';
		var body         = {
	      draw:       settings.draw, 
		  start:      settings.start, 
		  length:     settings.length,
		  column:     settings.order[0].column,
		  columnName: settings.columns[settings.order[0].column].data,
		  direction:  settings.order[0].dir,
		  search:     settings.search.value
	    };
		  
		var method = 'POST';
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method,
		  body: JSON.stringify(body)
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			fulfill(result);
          })
		  .catch(function(result) {
		    console.log(result);
		    reject(result);
          });
	  });
	}
	
    bucketLocations(data, type) {
	  return new Promise(function (fulfill, reject) {		
		var pathTemplate = AwsConstants.invokeUrl + '/api/bucketLocations';
		var body         = {
	      id: data,
		  type: type
	    };
		  
		var method = 'POST';
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method,
		  body: JSON.stringify(body)
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			fulfill(result);
          })
		  .catch(function(result) {
		    console.log(result);
		    reject(result);
          });
	  });
	}
	
	bucketScreenshots(data, type) {
	  return new Promise(function (fulfill, reject) {		
		var pathTemplate = AwsConstants.invokeUrl + '/api/screenshots';
		var body         = {
	      id: data,
		  type: type
	    };
		  
		var method = 'POST';
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method,
		  body: JSON.stringify(body)
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			fulfill(result);
          })
		  .catch(function(result) {
		    console.log(result);
		    reject(result);
          });
	  });
	}
	
	stats() {
	  return new Promise(function (fulfill, reject) {
	    var pathTemplate = AwsConstants.invokeUrl + '/api/stats';
		var method       = 'GET';
		  
		var request = new Request(pathTemplate, {
	      headers: new Headers({
		    'Content-Type' : 'application/json',
		    'Authorization': idToken()
	      }),
		  method: method
	    });
		
		fetch(request)
		  .then(function(result) {
			return result.json();
		  })
		  .then(function(result) {
			var push = [];
		  	for (var i = 0; i < result.length; i++) {
			  var s = result[i];
			  var obj  = {};
			  obj['title']     = s.source;
			  obj['objects']   = s.objects;
			  obj['links']     = s.totalLinks;
			  obj['invalid']   = s.invalidLinks;
			  obj['html']      = s.htmlFiles;
			  obj['extracted'] = s.lastExtracted;
			  obj['checked']   = s.lastChecked;
				  
			  push.push(obj);
			}
			
			fulfill(push);
          })
		 .catch(function(result) {
		    console.log(result);
			reject(result);
         });
	  });
	}
	
	configureScrenshotLinks() {
		
	  var that = this;

	  // Match to Bootstraps data-toggle for the modal
	  // and attach an onclick event handler
	  $('a.btn-info,a.label').off().on('click', function (e) {

		if ($(this).hasClass("selected") || $(this).closest("tr").hasClass("selected")) {
			$(this).removeClass("selected");
		}
		else {
			$('tr.selected').removeClass('selected');
			if ($(this).is("a")) {
				$(this).closest("tr").addClass("selected");
			} else {
				$(this).addClass("selected");
			}
		}
		
		var target_modal   = $(e.currentTarget).data('target');
		var remote_content = $(e.currentTarget).data('src');
		var remote_type    = $(e.currentTarget).data('type');
		var $modal         = $(target_modal);
		var $modalBody     = $(target_modal + ' .modal-body');

		if ($(this).hasClass("bucket-locations")) {

		  $modal.off().on('show.bs.modal', function () {
			$modalBody.empty();

			that.bucketLocations(remote_content, remote_type).then(function(d) {
			  var str = '<ul>';
			  $.each( d, function( key, value ) {
			    str += '<li><a href="' + value.data + '" target="_blank">' + value.data + '</a></li>';
              });
			  str += '</ul>';
			  if (d.length === 0) {
			    str = 'No Results';
		      }
			  $modalBody.html(str);
			});
		  }).modal();
			
		  return false;
		}
		
		if ($(this).hasClass("bucket-screenshots")) {

		  $modal.off().on('show.bs.modal', function () {
			$modalBody.empty();

			that.bucketScreenshots(remote_content, remote_type).then(function(d) {
			  var str = '<ul>';
			  $.each( d.urls, function( key, value ) {
			    str += '<li><img src="' + value.s_original + '"></img></li>';
              });
			  str += '</ul>';
			  if (d.length === 0) {
			    str = 'No Results';
		      }
			  $modalBody.html(str);
			});
		  }).modal();
			
		  return false;
		}

		return true;
	  });
    }
  }

  return ServicesBase;
}
