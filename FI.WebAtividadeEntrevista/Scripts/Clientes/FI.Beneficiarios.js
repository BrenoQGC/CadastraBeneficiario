$(document).ready(function () {

    if (document.getElementById("gridBeneficiarios"))
        $('#gridBeneficiarios').jtable({
            title: 'Beneficiarios',
            paging: true, //Enable paging
            pageSize: 5, //Set page size (default: 10)
            sorting: true, //Enable sorting
            defaultSorting: 'Nome ASC', //Set default sorting
            actions: {
                listAction: function (postData, jtParams) {
                    return $.Deferred(function ($dfd) {
                        $.ajax({
                            url: '/Cliente/BeneficiarioList',
                            type: 'POST',
                            dataType: 'json',
                            data: { id: data.record.Id },
                            success: function (data) {
                                $dfd.resolve({
                                    Result: 'OK',
                                    Records: data.Records,
                                    TotalRecordCount: data.TotalRecordCount
                                });
                            },
                            error: function () {
                                $dfd.reject();
                            }
                        });
                    });
                },
            },
            fields: {
                Nome: {
                    title: 'Nome',
                    width: '50%'
                },
                CPF: {
                    title: 'CPF',
                    width: '35%'
                },
                Alterar: {
                    title: '',
                    display: function (data) {
                        return '<button class="btn btn-primary btn-sm">Alterar</button>';
                    },
                    event: {
                        'click': function (event, data) {
                            editBeneficiario(event.currentTarget);
                        }
                    }
                }
            }

        });

    if (document.getElementById("gridBeneficiarios"))
        $('#gridBeneficiarios').jtable('load');

    function editBeneficiario(button) {
        $(button).hide();
        var row = $(button).closest('tr');
        var nomeField = row.find('td:nth-child(1)');
        var cpfField = row.find('td:nth-child(2)');
        nomeField.attr('contenteditable', true);
        cpfField.attr('contenteditable', true);
        var salvarButton = $('<button class="btn btn-primary btn-sm">Salvar</button>');
        var cancelarButton = $('<button class="btn btn-default btn-sm">Cancelar</button>');
        salvarButton.click(function () {
            saveBeneficiario(row);
        });
        cancelarButton.click(function () {
            cancelEditBeneficiario(row, nomeField, cpfField);
        });
        row.append($('<td>').append(salvarButton).append(cancelarButton));
    }

    function saveBeneficiario(row) {
        var id = row.attr('data-record-key');
        var nomeField = row.find('td:nth-child(1)');
        var cpfField = row.find('td:nth-child(2)');
        var nome = nomeField.text();
        var cpf = cpfField.text();
        $.ajax({
            url: '/Cliente/SalvarBeneficiario',
            type: 'POST',
            dataType: 'json',
            data: { id: id, nome: nome, cpf: cpf },
            success: function () {
                nomeField.attr('contenteditable', false);
                cpfField.attr('contenteditable', false);
                row.find('td:last-child').remove();
                row.find('td:nth-child(3)').show();
                $('#gridBeneficiarios').jtable('reload');
            },
            error: function () {
                alert('Erro ao salvar beneficiário');
            }
        });
    }

    function cancelEditBeneficiario(row, nomeField, cpfField) {
        nomeField.attr('contenteditable', false);
        cpfField.attr('contenteditable', false);
        row.find('td:last-child').remove();
        row.find('td:nth-child(3)').show();
    }

})
