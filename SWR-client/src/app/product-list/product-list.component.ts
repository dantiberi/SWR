import { Component, OnInit } from '@angular/core';
import { FetcherService } from '../fetcher.service';
import { Product } from '../ts/product';
import { ClipboardService } from 'ngx-clipboard';
import { HashTable } from 'angular-hashtable'; //https://github.com/brutalchrist/angular-hashtable
import { ThemePalette } from '@angular/material/core';
import { ProgressBarMode } from '@angular/material/progress-bar';
import { MatDialog } from '@angular/material/dialog';
import { ProductInfoComponent } from './product-info/product-info.component';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {


  private fetcherService: FetcherService; 
  public products: HashTable<string, Product> = new HashTable<string, Product>();
  public static clipboardApi: ClipboardService;

  constructor(public dialog: MatDialog, fetcherService: FetcherService, clipboardApi: ClipboardService){
    this.fetcherService = fetcherService;
    ProductListComponent.clipboardApi = clipboardApi;
  }
  
  /**
   * Processes each product in the products list into elements and adds them to the document. 
   */
  public async displayProductsInList(): Promise<void>{
    this.removeAllProductsFromDisplay();//Remove existing elements

    var arr: Product[] = this.iterableToArray(this.products); //Needed to reverse.
    arr = arr.reverse();
    console.log(arr);
    //this.products.forEach((key:string, p:Product) =>{
    arr.forEach((p: Product) => {
      
      var cardElement = document.createElement("mat-card");
      cardElement.setAttribute("id", "product:" + p.id);
      cardElement.setAttribute("class", "mat-card mat-focus-indicator");

      var imgElement = document.createElement("img");
      imgElement.setAttribute("src", p.img_url);
      imgElement.setAttribute("class", "product-img product-card-element");
      //imgElement.style.maxHeight = "10em"

      var nameElement = document.createElement("h3");
      nameElement.setAttribute("class", "product-name product-card-element");
      //nameElement.setAttribute("onclick", "window.location.href='" + p.url+"'")
      nameElement.innerText = p.name;

      var linkButtonElement = document.createElement("button");
      linkButtonElement.setAttribute("mat-button", "");
      linkButtonElement.setAttribute("_ngcontent-hgy-c46","");
      //linkButtonElement.setAttribute("(click)", "copyLink(" + p.url + ")");
      linkButtonElement.addEventListener('click', (e) => {ProductListComponent.copyLink(p.url)} );
      linkButtonElement.setAttribute("class", "product-link mat-focus-indicator mat-button mat-button-base product-card-element");
      linkButtonElement.innerText = "Copy Link";

      var removeButtonElement = document.createElement("button");
      removeButtonElement.setAttribute("mat-button", "");
      removeButtonElement.setAttribute("_ngcontent-hgy-c46","");
      //linkButtonElement.setAttribute("(click)", "copyLink(" + p.url + ")");
      removeButtonElement.addEventListener('click', (e) => {this.productInfoButton(p)} );
      removeButtonElement.setAttribute("class", "product-info mat-focus-indicator mat-button mat-button-base product-card-element");

      var removeButtonIcon = document.createElement("mat-icon");
      removeButtonIcon.innerText="info";
      removeButtonIcon.setAttribute("class", "mat-icon material-icons");
      removeButtonElement.appendChild(removeButtonIcon);

      var priceElement = document.createElement("h3");
      priceElement.setAttribute("class", "product-price product-card-element");
      priceElement.innerText = "$" + p.price;
      if(p.isOnSale == 1){
        priceElement.setAttribute("class", priceElement.getAttribute("class") + " price-on-sale");
        priceElement.innerText = "Discounted: " + priceElement.innerText;
        //console.log("PRODUCT ON SALE!")
      }

      cardElement.appendChild(nameElement);
      cardElement.appendChild(imgElement);
      cardElement.appendChild(linkButtonElement);
      cardElement.appendChild(priceElement);
      cardElement.appendChild(removeButtonElement);

      document.getElementById("product-list-wrapper")?.appendChild(cardElement); //? because it could be null

      //document.getElementById("product:" + p.id)?.appendChild(imgElement); //? because it could be null
    });
  }

  public iterableToArray(hash: HashTable<string, Product>): Product[]{
    var out: Product[] = [];
    for(var i = 0; i< hash.getKeys().length; i++){
      out = out.concat(hash.get(hash.getKeys()[i]));
    }
    return out;
  }

  /**
   * Clears the display but NOT the products list.
   */
  public removeAllProductsFromDisplay(): void{
    for(let i = 0; i < this.products.size(); i++){
      //document.getElementById("product:" + this.products.)?.remove();
      this.products.forEach((key: string, value: Product) => {
        document.getElementById("product:" + this.products.get("product:"+value.id).id)?.remove();
      });
    }
  }

  /**
   * Removes a product with the given id from the document.
   * @param id Product with this id to be removed.
   */
  public removeProductFromDisplay(id: number): void{
    //console.log("product:" + id);
    document.getElementById("product:" + id)?.remove();
  }

  /**
   * Calls for the product with given id to be removed from the products list and the document.
   * @param id Product with this id to be removed.
   */
   public async removeProduct(id: number){
    this.products.remove("product:"+id);
    this.removeProductFromDisplay(id);

    await this.fetcherService.deleteProduct(id).toPromise();
  }

  public productInfoButton(product: Product):void{
    this.dialog.open(ProductInfoComponent,{
      data: {
        product: product,
      },
      width: '50%',
      height: '60%',
    }).afterClosed()
    .subscribe(response => {
      //console.log(response);
      if(response != null && response.delete == true){
        this.removeProduct(response.product.id);
      }
    });
  }

  public async processProductJson(j: any): Promise<void>{
    var json: string = JSON.stringify(j);
    var pJson =  JSON.parse(json);
    //console.log(pJson.name + " | " + pJson.isOnSale)
    var p: Product = new Product(pJson.url, pJson.name, pJson.price, pJson.imgUrl, pJson.id, pJson.isOnSale);
    //this.products = this.products.concat(p);//Add to products list
    this.products.put("product:"+p.id, p);
  }

  /**
   * Loads all products from the server-side database into the products hashtable.
   */
  public async loadProducts(): Promise<boolean>{
    this.products = new HashTable<string, Product>();//Clear current products list;
    var res = await this.fetcherService.getAllProducts().toPromise();
    var tempString: string = JSON.stringify(res);
    var productListJSON =  JSON.parse(tempString);

    for(const j of Object.values(productListJSON)){
      await this.processProductJson(j);
    }
    Promise.resolve();
    return true;
  }

  /**
   * Called by event emitted from add-product-component to submit a product url to the backend.
   * @param url Link for the product to be added.
   */
  async registerProduct(url: string): Promise<void>{
    //console.log("REGISTER PRODUCT: " + url);
    try {
      if((url.includes(".com") || url.includes(".net") || url.includes(".org") || url.includes(".gov") || url.includes(".edu") || url.includes(".co") || url.includes(".uk")) && url.includes("http")){
        document.getElementsByClassName("add-progress-bar")[0].setAttribute("mode", "indeterminate")
        await this.fetcherService.giveAmazonProduct(new Product(url, "", -1.0, "", -1, 0)).toPromise();
        document.getElementsByClassName("add-progress-bar")[0].setAttribute("mode", "determinate")
      }
    } catch (error) {
      console.log(error);
    }

    this.loadAndDisplay();
  }

  /**
   * Debug function used to retrieve the number of elemtns in the the products hashtable.
   */
  public productsLength(): void{
    console.log("Num products: " + this.products.getAll().length);
  }

  /**
   * Calls for the products to be both loaded from the server database into the products hashtable and
   * render them to the document. 
   */
  async loadAndDisplay(): Promise<void>{
    //console.log("loadAndDisplay() CALLED");
    await this.loadProducts();
    this.displayProductsInList();
  }

  /**
   * Copies the given string, in this case a URL, to the clipboard.
   * @param url String to be copied.
   */
  static copyLink(url: string): void{
    this.clipboardApi.copyFromContent(url);
  }

  ngAfterViewInit(){
    this.loadAndDisplay(); //Makes sure the products are loaded and displayed at startup.
  }

  ngOnInit(): void{
    
  }

}
