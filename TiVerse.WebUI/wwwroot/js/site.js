﻿$(document).ready(function () {

    const DEFAULT_TRANSPORT = 'bus';
    const SLIDER_OPTIONS = {
        start: [0, maxPrice],
        connect: true,
        step: 1,
        range: {
            'min': 0,
            'max': maxPrice
        }
    };

    let selectedTransport = '';
    let pageNumber = 1;
    let sortingParameter = $('input[type=radio][name=sortingParameter]:checked').val();
    let sortOrder = $('input[type=radio][name=sortingOrder]:checked').val();
    let isLoading = false;

    let priceRange = { min: 0, max: maxPrice };

    const rangeSlider = document.getElementById('range-slider');
    const inputMin = document.getElementById('input-0');
    const inputMax = document.getElementById('input-1');

    // Initializing default Parameters
    const urlParams = new URLSearchParams(window.location.search);
    selectedTransport = urlParams.get('Transport') || DEFAULT_TRANSPORT;

    // Function of sending an AJAX request when changing radio buttons
    function sendRequestOnRadioChange() {
        sortingParameter = $('input[type=radio][name=sortingParameter]:checked').val();
        sortOrder = $('input[type=radio][name=sortingOrder]:checked').val();
        pageNumber = 1;

        loadRoutes(selectedTransport, sortingParameter, sortOrder, pageNumber, priceRange.min, priceRange.max);
    }

    // Route loading function
    function loadRoutes(selectedTransport, sortingCriteria, sortOrder, page, minPrice, maxPrice, selectedCities) {
        if (isLoading) return;

        isLoading = true;

        $.ajax({
            url: '/Transport/LoadRoutes',
            type: 'Post',
            data: {
                page: page,
                selectedTransport: selectedTransport,
                sortingCriteria: sortingCriteria,
                sortOrder: sortOrder,
                minPrice: minPrice,
                maxPrice: maxPrice,
                selectedCities: selectedCities
            },
            success: function (result) {
                if (page === 1) {
                    $('#routeContainer').html(result);
                } else {
                    $('#routeContainer').append(result);
                }
            },
            complete: function () {
                isLoading = false;
            },
            error: function () {
                console.log('Ошибка при загрузке маршрутов');
            }
        });
    }

    // Function of sending an AJAX request when changing checkbox buttons
    function sendRequestOnCheckboxChange() {
        sortingParameter = $('input[type=radio][name=sortingParameter]:checked').val();
        sortOrder = $('input[type=radio][name=sortingOrder]:checked').val();
        pageNumber = 1;

        const selectedCities = [];
        $('input[name="cityCheckbox"]:checked').each(function () {
            selectedCities.push($(this).val());
        });

        loadRoutes(selectedTransport, sortingParameter, sortOrder, pageNumber, priceRange.min, priceRange.max, selectedCities);
    }

    // Popular routes link ecent handler
    $('.routes-container__link').on('click', function (e) {
        e.preventDefault();
        var url = $(this).data('url');
        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                $('.maincontent').html(data);
            },
            error: function () {
                console.log('Error occurred while fetching data.');
            }
        });
    });

    // Radio button change event handler
    $('input[type=radio]').change(sendRequestOnRadioChange);

    // Page scroll event handler
    $(window).scroll(function () {
        if ($(window).scrollTop() + $(window).height() >= $(document).height() - 100 && !isLoading) {
            pageNumber++;
            loadRoutes(selectedTransport, sortingParameter, sortOrder, pageNumber, priceRange.min, priceRange.max);
        }
    });

    // Transport selection handler
    $('#second-section__transport-selector').change(function () {
        selectedTransport = $(this).val() || DEFAULT_TRANSPORT;
        window.location.href = '/Transport/Index?Transport=' + selectedTransport;
    });

    $('#second-section__language-selector').change(function (e) {
        var selectedCulture = e.target.value; 
        fetch(`/SetCulture?culture=${selectedCulture}`, { method: 'POST' })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                location.reload();
            })
            .catch(error => {
                console.error('Error:', error);
            });
    });


    // Event handler for changing checkboxes with cities
    $('input[name="cityCheckbox"]').change(sendRequestOnCheckboxChange);


    // Slider for selection by price
    if (rangeSlider) {
        noUiSlider.create(rangeSlider, SLIDER_OPTIONS);

        rangeSlider.noUiSlider.on('change', function (values, handle) {
            const minValue = parseFloat(values[0]);
            const maxValue = parseFloat(values[1]);

            inputMin.value = minValue;
            inputMax.value = maxValue;

            priceRange.min = minValue;
            priceRange.max = maxValue;

            $.ajax({
                url: '/Transport/LoadRoutes',
                type: 'POST',
                data: {
                    page: pageNumber,
                    selectedTransport: selectedTransport,
                    sortingCriteria: sortingParameter,
                    sortOrder: sortOrder,
                    minPrice: priceRange.min,
                    maxPrice: priceRange.max
                },
                success: function (data) {
                    $('#routeContainer').html(data);
                },
                error: function () {
                    console.log('Error occurred while sending slider values.');
                }
            });
        });
    }

    // Choosing page in profile 
    $('#personalInfoLink, #plannedTripLink, #tripHistoryLink, #accountBalanceLink').on('click', function (e) {
        e.preventDefault();

        var partialViewName = $(this).data('partialview');

        $.ajax({
            url: '/Account/LoadPartialView',
            type: 'GET',
            data: { viewName: partialViewName },
            success: function (data) {
                $('.displaypage').html(data);
            },
            error: function () {
                alert('Ошибка при загрузке данных.');
            }
        });
    });

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/messagehub")
        .build();

    connection.on("MessageCreated", function (model) {
        console.log("Новий маршрут створений");
        toastr.success("Новий маршрут створений");
    });

    connection.start()
        .then(function () {
            console.log("Подключение к SignalR установлено.");
        })
        .catch(function (err) {
            console.error("Ошибка при подключении к SignalR:", err.toString());
        });
});