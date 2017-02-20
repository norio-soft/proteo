function getTemplate(templateName) {
    if (Handlebars.templates === undefined || Handlebars.templates[templateName] === undefined) {
        $.ajax({
            url: '/templates/' + templateName + '.handlebars',
            success: function (data) {
                if (Handlebars.templates === undefined) {
                    Handlebars.templates = {};
                }

                Handlebars.templates[templateName] = Handlebars.compile(data);
            },
            async: false
        });
    }

    return Handlebars.templates[templateName];
};
