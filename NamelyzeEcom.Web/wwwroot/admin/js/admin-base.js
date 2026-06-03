/**
 * NamelyzeEcom Admin Base System v2.0
 * Provides: Sidebar injection, CRUD engine, DataTable factory,
 *           Toast notifications, Confirm dialogs, Image preview
 *
 * Usage: Include this file in every admin page.
 * Config: Set window.AdminPage = { title, pageId } before including.
 */

(function (window, $) {
    'use strict';

    /* =====================================================
       CONFIG & STATE
       ===================================================== */
    const API_BASE = '/api';
    let _pendingCount = 0;

    /* =====================================================
       SIDEBAR DEFINITION
       ===================================================== */
    const SIDEBAR_MENU = [
        { section: 'Main' },
        { id: 'dashboard', label: 'Dashboard', icon: 'tachometer-alt', href: '/Admin/Index', badge: null },

        { section: 'Catalog' },
        { id: 'products', label: 'Products', icon: 'box', href: '/Admin/Products' },
        { id: 'categories', label: 'Categories', icon: 'th-large', href: '/Admin/Categories' },
        { id: 'brands', label: 'Brands', icon: 'certificate', href: '/Admin/Brands' },

        { section: 'Sales' },
        { id: 'orders', label: 'Orders', icon: 'shopping-cart', href: '/Admin/Orders', badge: 'pending' },
        { id: 'customers', label: 'Customers', icon: 'users', href: '/Admin/Customers' },
        { id: 'sellers', label: 'Sellers', icon: 'store', href: '/Admin/Sellers' },
        { id: 'coupons', label: 'Coupons', icon: 'percent', href: '/Admin/Coupons' },

        { section: 'Content' },
        { id: 'banners', label: 'Banners/Sliders', icon: 'images', href: '/Admin/Banners' },
        { id: 'blogs', label: 'Blogs', icon: 'blog', href: '/Admin/Blogs' },
        { id: 'policies', label: 'Policies & Rules', icon: 'file-contract', href: '/Admin/Policies' },

        { section: 'Inventory' },
        { id: 'inventory', label: 'Inventory', icon: 'warehouse', href: '/Admin/Inventory' },

        { section: 'System' },
        { id: 'users', label: 'Admin Users', icon: 'user-shield', href: '/Admin/Users' },
        { id: 'settings', label: 'Settings', icon: 'cog', href: '/Admin/Settings' },
    ];

    /* =====================================================
       SIDEBAR RENDER
       ===================================================== */
    function renderSidebar() {
        const currentPage = (window.AdminPage && window.AdminPage.pageId) || '';
        let html = `
        <div id="adminSidebar">
            <div class="sb-brand">
                <div class="sb-logo"><i class="fas fa-shopping-bag"></i></div>
                <div class="sb-title">NamelyzeEcom <small>Admin Panel</small></div>
            </div>`;

        SIDEBAR_MENU.forEach(function (item) {
            if (item.section) {
                html += `<div class="sb-section">${item.section}</div><ul class="sb-menu">`;
                return;
            }
            const active = (item.id === currentPage) ? 'active' : '';
            const badgeHtml = item.badge === 'pending'
                ? `<span class="sb-count" id="sbBadge_${item.id}">…</span>` : '';
            html += `
                <li class="${active}">
                    <a href="${item.href}">
                        <span class="sb-icon"><i class="fas fa-${item.icon}"></i></span>
                        ${item.label} ${badgeHtml}
                    </a>
                </li>`;
        });
        // close last <ul> section
        html += `</ul>
            <div class="sb-footer">
                <div class="sb-user">
                    <div class="av">A</div>
                    <div>
                        <div class="name">Super Admin</div>
                        <div class="role">Administrator</div>
                    </div>
                    <a href="/Account/Login" class="ms-auto" title="Logout"
                       style="color:rgba(255,255,255,.4);font-size:14px;">
                        <i class="fas fa-sign-out-alt"></i></a>
                </div>
            </div>
        </div>`;
        return html;
    }

    /* =====================================================
       TOPBAR RENDER
       ===================================================== */
    function renderTopbar() {
        const page = window.AdminPage || {};
        return `
        <div class="topbar">
            <div>
                <div class="tb-title">${page.title || 'Dashboard'}</div>
                <div class="tb-sub">Admin &rsaquo; ${page.title || 'Dashboard'}</div>
            </div>
            <div class="topbar-right">
                <div class="tb-btn" title="Notifications" id="tbNotifBtn">
                    <i class="fas fa-bell"></i>
                    <span class="tb-notif-badge" id="tbNotifCount" style="display:none">0</span>
                </div>
                <div class="tb-btn" title="Visit Site" onclick="window.open('/')">
                    <i class="fas fa-external-link-alt"></i>
                </div>
                <div class="tb-profile" onclick="window.location='/Account/Login'">
                    <div class="av">A</div>
                    <span>Logout</span>
                    <i class="fas fa-sign-out-alt" style="font-size:11px;opacity:.5"></i>
                </div>
            </div>
        </div>`;
    }

    /* =====================================================
       INIT — Inject layout into page
       ===================================================== */
    function init() {
        const $app = $('#adminApp');
        if (!$app.length) {
            // If we are in a Razor layout, .admin-wrapper should already exist
            if ($('.admin-wrapper').length) {
                loadPendingCount();
            }
            return;
        }

        // Legacy/Static support: Inject layout if #adminApp exists and isn't nested in .admin-wrapper
        if (!$app.closest('.admin-wrapper').length) {
            const pageContent = $app.html();
            $app.html(`
                <div class="admin-wrapper">
                    ${renderSidebar()}
                    <div class="main-area">
                        ${renderTopbar()}
                        <div class="content-area">
                            ${pageContent}
                        </div>
                    </div>
                </div>`);
        }

        // Load pending orders count for badge
        loadPendingCount();
    }

    /* =====================================================
       PENDING COUNT (for sidebar badge)
       ===================================================== */
    function loadPendingCount() {
        $.get(API_BASE + '/orders?status=1', function (res) {
            var data = res.data || res;
            var count = Array.isArray(data) ? data.length : (res.pendingCount || 0);
            if (count > 0) {
                $('#sbBadge_orders').text(count);
                $('#tbNotifCount').text(count).show();
            }
        }).fail(function () { /* silently fail */ });
    }

    /* =====================================================
       TOAST NOTIFICATION
       ===================================================== */
    function toast(msg, type) {
        type = type || 'success';
        const colors = { success: '#22c55e', error: '#ef4444', warning: '#f59e0b', info: '#3b82f6' };
        const icons = { success: 'check-circle', error: 'times-circle', warning: 'exclamation-triangle', info: 'info-circle' };

        const id = 'toast_' + Date.now();
        const $t = $(`
            <div id="${id}" style="
                position:fixed;top:78px;right:20px;z-index:9999;
                background:#fff;border:1px solid #e5e9f2;
                border-left:4px solid ${colors[type]};
                border-radius:8px;padding:12px 18px;
                box-shadow:0 4px 20px rgba(0,0,0,.1);
                display:flex;align-items:center;gap:10px;
                min-width:260px;animation:fadeInRight .3s;
                font-size:13px;font-weight:500;
            ">
                <i class="fas fa-${icons[type]}" style="color:${colors[type]};font-size:16px;"></i>
                ${msg}
                <i class="fas fa-times ms-auto" style="cursor:pointer;color:#aaa;font-size:12px;"
                   onclick="$(this).closest('[id^=toast_]').remove()"></i>
            </div>`);
        $('body').append($t);
        setTimeout(function () { $t.fadeOut(300, function () { $t.remove(); }); }, 4000);
    }

    /* =====================================================
       CONFIRM DIALOG
       ===================================================== */
    function confirm(msg, onYes) {
        if (window.Swal) {
            Swal.fire({
                title: 'Are you sure?',
                text: msg,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#FE5200',
                cancelButtonColor: '#6b7280',
                confirmButtonText: 'Yes, proceed!',
                cancelButtonText: 'Cancel'
            }).then(function (r) { if (r.isConfirmed) onYes(); });
        } else {
            if (window.confirm(msg)) onYes();
        }
    }

    /* =====================================================
       CRUD ENGINE
       ===================================================== */
    const Crud = {
        /**
         * Generic GET list
         * @param {string} endpoint
         * @param {function} callback receives array of data
         */
        getList: function (endpoint, callback) {
            $.ajax({
                url: API_BASE + endpoint,
                method: 'GET',
                success: function (res) {
                    callback(null, res.data || res || []);
                },
                error: function (xhr) {
                    callback(xhr.responseJSON || 'Error fetching data', []);
                }
            });
        },

        /**
         * Create record
         * @param {string} endpoint
         * @param {object} data
         * @param {function} callback
         */
        create: function (endpoint, data, callback) {
            $.ajax({
                url: API_BASE + endpoint,
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (res) { callback(null, res.data || res); },
                error: function (xhr) { callback(xhr.responseJSON || 'Error creating record'); }
            });
        },

        /**
         * Update record
         */
        update: function (endpoint, id, data, callback) {
            $.ajax({
                url: API_BASE + endpoint + '/' + id,
                method: 'PUT',
                contentType: 'application/json',
                data: JSON.stringify(data),
                success: function (res) { callback(null, res.data || res); },
                error: function (xhr) { callback(xhr.responseJSON || 'Error updating record'); }
            });
        },

        /**
         * Delete record
         */
        delete: function (endpoint, id, callback) {
            $.ajax({
                url: API_BASE + endpoint + '/' + id,
                method: 'DELETE',
                success: function (res) { callback(null, res); },
                error: function (xhr) { callback(xhr.responseJSON || 'Error deleting record'); }
            });
        }
    };

    /* =====================================================
       DATATABLE FACTORY
       Creates a standard DataTable with search, page size, export
       ===================================================== */
    function makeTable(selector, columns, data) {
        if ($.fn.DataTable.isDataTable(selector)) {
            $(selector).DataTable().destroy();
        }
        return $(selector).DataTable({
            data: data,
            columns: columns,
            responsive: true,
            pageLength: 25,
            order: [[0, 'desc']],
            dom: '<"a-table-head"<"d-flex align-items-center gap-2"l<"ms-auto"f>>>rt<"d-flex justify-content-between align-items-center p-3"ip>',
            language: {
                search: '',
                searchPlaceholder: 'Search...',
                lengthMenu: '_MENU_ per page',
                info: 'Showing _START_-_END_ of _TOTAL_',
                paginate: { previous: '←', next: '→' }
            }
        });
    }

    /* =====================================================
       IMAGE PREVIEW HELPER
       ===================================================== */
    function bindImagePreview(inputSel, previewSel) {
        $(document).on('change', inputSel, function () {
            const file = this.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (e) { $(previewSel).attr('src', e.target.result).show(); };
                reader.readAsDataURL(file);
            }
        });
    }

    /* =====================================================
       FORM HELPERS
       ===================================================== */
    function serializeForm(formSel) {
        const obj = {};
        $(formSel).serializeArray().forEach(function (f) {
            // handle checkboxes
            if (f.name.endsWith('[]')) {
                const key = f.name.slice(0, -2);
                obj[key] = obj[key] || [];
                obj[key].push(f.value);
            } else {
                obj[f.name] = f.value;
            }
        });
        // handle unchecked checkboxes (send false)
        $(formSel + ' input[type=checkbox]').each(function () {
            if (!$(this).is(':checked')) obj[this.name] = false;
            else obj[this.name] = true;
        });
        return obj;
    }

    function populateForm(formSel, data) {
        Object.keys(data).forEach(function (key) {
            const $el = $(formSel + ' [name="' + key + '"]');
            if ($el.is(':checkbox')) $el.prop('checked', !!data[key]);
            else $el.val(data[key]);
        });
    }

    function clearForm(formSel) {
        $(formSel)[0] && $(formSel)[0].reset();
        $(formSel + ' [name]').val('');
    }

    /* =====================================================
       NUMBER FORMAT
       ===================================================== */
    function currency(n) {
        return '৳ ' + Number(n || 0).toLocaleString('en-BD');
    }

    /* =====================================================
       STATUS BADGE HTML
       ===================================================== */
    const STATUS_MAP = {
        1: { label: 'Pending', cls: 'pending' },
        2: { label: 'Confirmed', cls: 'confirm' },
        3: { label: 'Processing', cls: 'confirm' },
        4: { label: 'Shipped', cls: 'confirm' },
        5: { label: 'Out for Del.', cls: 'confirm' },
        6: { label: 'Delivered', cls: 'deliver' },
        7: { label: 'Cancelled', cls: 'cancel' },
        8: { label: 'Returned', cls: 'cancel' },
    };
    function statusBadge(s) {
        const m = STATUS_MAP[s] || { label: 'Unknown', cls: 'pending' };
        return `<span class="s-badge ${m.cls}">${m.label}</span>`;
    }
    function activeBadge(b) {
        return b ? `<span class="s-badge active">Active</span>` : `<span class="s-badge inactive">Inactive</span>`;
    }

    /* =====================================================
       EXPORT PUBLIC API
       ===================================================== */
    window.AdminBase = {
        init: init,
        toast: toast,
        confirm: confirm,
        Crud: Crud,
        makeTable: makeTable,
        serializeForm: serializeForm,
        populateForm: populateForm,
        clearForm: clearForm,
        currency: currency,
        statusBadge: statusBadge,
        activeBadge: activeBadge,
        bindImagePreview: bindImagePreview,
        API: API_BASE,

        /**
         * Upload a file to the server
         * @param {File} file 
         * @param {function} callback returns {url, success}
         */
        uploadFile: function (file, callback) {
            const formData = new FormData();
            formData.append('file', file);
            $.ajax({
                url: API_BASE + '/media/upload',
                method: 'POST',
                data: formData,
                processData: false,
                contentType: false,
                success: function (res) { callback(null, res); },
                error: function (xhr) { callback(xhr.responseJSON || 'Upload failed'); }
            });
        },

        /**
         * Attaches a hidden file input to a text input (URL) to allow local upload
         * @param {string} inputSelector e.g. '#imgUrl'
         * @param {string} previewSelector e.g. '#imgPreview'
         */
        initImageUpload: function (inputSelector, previewSelector) {
            const $input = $(inputSelector);
            const $preview = $(previewSelector);
            const id = 'file_' + Math.random().toString(36).substr(2, 9);
            const $fileInput = $(`<input type="file" id="${id}" style="display:none" accept="image/*">`);
            const $btn = $(`<button type="button" class="btn btn-sm btn-outline-secondary mt-1" style="font-size:11px;">
                <i class="fas fa-upload me-1"></i>Upload Local
            </button>`);

            $input.after($btn);
            $btn.after($fileInput);

            $btn.on('click', () => $fileInput.trigger('click'));

            $fileInput.on('change', function () {
                const file = this.files[0];
                if (!file) return;
                $btn.html('<i class="fas fa-spinner fa-spin"></i> Uploading...');
                AdminBase.uploadFile(file, function (err, res) {
                    $btn.html('<i class="fas fa-upload me-1"></i>Upload Local');
                    if (err) { AdminBase.toast(err, 'error'); return; }
                    $input.val(res.url).trigger('input');
                    if ($preview.length) $preview.attr('src', res.url).show();
                    AdminBase.toast('Image uploaded!', 'success');
                });
            });
        }
    };

    // Auto-init on DOM ready if adminApp exists
    $(function () { if ($('#adminApp').length) init(); });

}(window, jQuery));
