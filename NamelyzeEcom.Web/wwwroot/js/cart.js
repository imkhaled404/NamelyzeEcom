const CartManager = {
    _key: 'namelyze_cart',

    get: function () {
        const data = localStorage.getItem(this._key);
        return data ? JSON.parse(data) : [];
    },

    save: function (items) {
        localStorage.setItem(this._key, JSON.stringify(items));
        this.updateUI();
    },

    add: function (product, quantity = 1) {
        let items = this.get();
        const existing = items.find(i => i.id === product.id);
        if (existing) {
            existing.quantity += quantity;
        } else {
            items.push({
                id: product.id,
                name: product.name,
                price: product.salePrice || product.regularPrice,
                image: product.mainImageUrl || 'https://placehold.co/100',
                slug: product.slug,
                quantity: quantity
            });
        }
        this.save(items);
        NamelyzeEcom.toast('সফল!', 'পণ্যটি কার্টে যোগ করা হয়েছে।', 'success');
    },

    remove: function (productId) {
        let items = this.get();
        items = items.filter(i => i.id !== productId);
        this.save(items);
    },

    clear: function () {
        this.save([]);
    },

    count: function () {
        return this.get().reduce((sum, item) => sum + item.quantity, 0);
    },

    total: function () {
        return this.get().reduce((sum, item) => sum + (item.price * item.quantity), 0);
    },

    updateUI: function () {
        const count = this.count();
        $('.badge-count').text(count);
        if (count > 0) $('.badge-count').fadeIn();
        else $('.badge-count').fadeOut();
    },

    addToCartAndCheckout: function (product) {
        this.add(product);
        window.location.href = '/Home/Checkout';
    }
};

$(document).ready(() => CartManager.updateUI());
