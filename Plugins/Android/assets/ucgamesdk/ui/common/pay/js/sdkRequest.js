var sdkRequest={rsaSign:function(e,t,n){var r=new sdkBase;r.request(r.servType.COMMON,r.actions.SIGN,{data:e,mode:"RSA",ver:"01"},t,n,!0,900)},rsaEncrypt:function(e,t){var n=RSA.getPublicKey("-----BEGIN PUBLIC KEY-----MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC7+1UFF73z/61uA1jbYW7VWVwUfS/8onM/alW4jddA5che3MZoV/4J4V49S+iYzzdvrqobAhWQA0kTsN3l4nEAnFKYKXx+0JWBZAymakMbedcXK3mvPsN8OFzQgV/Gpk1YL9gvncVCUFDtPpjLKebdXhVD1Sb7LGP2ycO2cqgHRwIDAQAB-----END PUBLIC KEY-----"),r=RSA.encrypt(encodeURIComponent(e),n);return r},sendReq:function(e,t,n,r){var i=JSON.stringify(t);sdkRequest.rsaSign(i,function(i){sdkRequest.sendAfterRsaSign(e,t,i.data.result,n,r)},r)},sendAfterRsaSign:function(e,t,n,r,i){var s=JSON.stringify(t),o,u=!0;try{o=sdkRequest.rsaEncrypt(s)}catch(a){u=!1,o=encodeURIComponent(s)}$.ajax({type:"GET",url:e,data:{reqData:o,encrypt:u,t:(new Date).getTime(),sign:n},timeout:1e4,dataType:"jsonp",success:r,error:i})},signAppendData:function(e,t){var n=JSON.stringify(e),r="",i="",s=!0,o="",u=(new Date).getTime();try{i=sdkRequest.rsaEncrypt(n)}catch(a){s=!1,i=encodeURIComponent(n)}sdkRequest.rsaSign(n,function(e){o=e.data.result,r="reqData="+i+"&encrypt="+s+"&sign="+o+"&t="+u,t(r)},function(){r="reqData="+i+"&encrypt="+s+"&sign="+o+"&t="+u,t(r)})}}