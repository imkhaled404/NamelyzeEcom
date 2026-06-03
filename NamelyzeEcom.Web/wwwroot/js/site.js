const API_BASE_URL = '/api';

const NamelyzeEcom = {
    // Auth helpers
    getToken: () => localStorage.getItem('jwt_token'),
    setToken: (token) => localStorage.setItem('jwt_token', token),
    removeToken: () => localStorage.removeItem('jwt_token'),

    // AJAX Wrapper
    ajax: function (url, method, data, successCallback, errorCallback) {
        const token = this.getToken();
        $.ajax({
            url: API_BASE_URL + url,
            method: method,
            data: data ? JSON.stringify(data) : null,
            contentType: 'application/json',
            headers: token ? { 'Authorization': 'Bearer ' + token } : {},
            success: function (response) {
                if (response.success) {
                    if (successCallback) successCallback(response.data, response.message);
                } else {
                    if (errorCallback) errorCallback(response.message, response.errors);
                    else NamelyzeEcom.toast('Error', response.message, 'error');
                }
            },
            error: function (xhr) {
                console.error(xhr);
                let msg = 'Something went wrong';
                if (xhr.status === 401) {
                    msg = 'Session expired. Please login again.';
                    NamelyzeEcom.removeToken();
                    // window.location.href = '/login.html';
                }
                if (errorCallback) errorCallback(msg);
                else NamelyzeEcom.toast('Error', msg, 'error');
            }
        });
    },

    // Toast Notifications using SweetAlert2
    toast: function (title, text, icon = 'info') {
        Swal.fire({
            title: title,
            text: text,
            icon: icon,
            timer: 3000,
            showConfirmButton: false,
            toast: true,
            position: 'top-end'
        });
    },

    // Direct Order (Bypass Cart for High Conversion)
    directOrder: function (productId) {
        // Simple implementation: navigate to checkout with product ID
        window.location.href = `/checkout.html?productId=${productId}`;
    },

    // Format Currency
    formatCurrency: function (amount) {
        return '৳' + parseFloat(amount).toLocaleString();
    },

    // WhatsApp Order Link Helper
    openWhatsApp: function (productName, price) {
        const phone = '8801804193796'; // Updated Hotline
        const msg = `আসসালামু আলাইকুম, আমি "${productName}" পণ্যটি অর্ডার করতে চাই। মূল্য: ${NamelyzeEcom.formatCurrency(price)}। প্রোডাক্ট লিঙ্ক: ${window.location.href}`;
        const url = `https://wa.me/${phone}?text=${encodeURIComponent(msg)}`;
        window.open(url, '_blank');
    }
};

// Global initializers
$(document).ready(function () {
    // Tooltip init
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    });

    // Cart Button Update
    $(document).on('click', '.btn-order', function () {
        const id = $(this).data('id');
        NamelyzeEcom.directOrder(id);
    });
});
