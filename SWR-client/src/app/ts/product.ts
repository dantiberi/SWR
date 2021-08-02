export class Product {
    public url: string = "";
    public name: string = "";
    public price: number = -0.00;
    public img_url: string = "";
    public id: number = -1;
    public isOnSale: number = 0;

    constructor(url: string, name: string, price: number, img_url: string, id:number, isOnSale: number){
        this.url = url;
        this.name = name;
        this.price = price;
        this.img_url = img_url;
        this.id = id;
        this.isOnSale = isOnSale;
    }
}