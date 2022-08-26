(function () {
    let forms = document.getElementsByTagName('form');
    if (forms.length < 1)
        return null;

    let form = forms[0];
    if (form.hasAttribute('method')
        && form.method === 'post'
        && form.hasAttribute('action')
        && form.action === '%%CALLBACKURL%%') {
        let inputElements = document.getElementsByTagName('input');
        let result = [];

        for (let i = 0; i < inputElements.length; i++) {
            let name = inputElements[i].getAttribute('name');
            let value = inputElements[i].getAttribute('value');

            result.push(encodeURIComponent(name) + '=' + encodeURIComponent(value));
        }
        return '?' + result.join('&');
    }
    return null;
})();