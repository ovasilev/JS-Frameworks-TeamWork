var PageManager = Class.create(function (count) {
    this.data;
    this.pageData;
    this.count = count;
    this.currentPage = 0;
    this.lastPage;
    this.total;
}, {
    loadData: function (data) {
        this.data = data;
        this.total = data.length;
        this.currentPage = 0;
        this.lastPage = Math.ceil(this.total / this.count);
        this.load();
    },
    load: function () {
        this.pageData = _.first(_.rest(this.data, this.currentPage * this.count), this.count)
    },
    nextPage: function () {
        if (this.currentPage < this.lastPage - 1) {
            this.currentPage++;
            this.load();
        }
    },
    prevPage: function () {
        if (this.currentPage > 0) {
            this.currentPage--;
            this.load();
        }
    }
});