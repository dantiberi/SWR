export class Product {
    public url: string = "";
    public name: string = "";
    public price: number = -0.00;

    constructor(url: string, name: string, price: number){
        this.url = url;
        this.name = name;
        this.price = price;
    }
}