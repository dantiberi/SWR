import { Component, OnInit } from '@angular/core';
import { FetcherService } from '../fetcher.service';
import { Product } from '../ts/product';
import { ClipboardService } from 'ngx-clipboard';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {


  private fetcherService: FetcherService; 
  public products: Product[] = [];
  private clipboardApi: ClipboardService;

  constructor(fetcherService: FetcherService, clipboardApi: ClipboardService){
    //fetcherService.getTestProdcut("https://www.amazon.com/Apple-AirPods-Charging-Latest-Model/dp/B07PXGQC1Q/ref=sr_1_1_sspa?dchild=1&keywords=airpods&qid=1627507012&sr=8-1-spons&psc=1&spLa=ZW5jcnlwdGVkUXVhbGlmaWVyPUExQ1BLQUwwTU9EU1VBJmVuY3J5cHRlZElkPUEwMzUyMTQxMVlDQkZPREZROElQMiZlbmNyeXB0ZWRBZElkPUEwMzA4NzczMkMyM1hTQVYyRjFMVyZ3aWRnZXROYW1lPXNwX2F0ZiZhY3Rpb249Y2xpY2tSZWRpcmVjdCZkb05vdExvZ0NsaWNrPXRydWU=").subscribe(response => {
    //var testProduct = new Product("https://www.amazon.com/JBL-Waterproof-Portable-Bluetooth-Speaker/dp/B07QK18BNY/?_encoding=UTF8&pd_rd_w=BD46g&pf_rd_p=620e7d0f-07bf-4434-8cbc-ed3abf0cf403&pf_rd_r=654DDX2X8RMC2QF9E003&pd_rd_r=aa8a8adb-73ef-47c9-ab0f-9c6569c95f3b&pd_rd_wg=BVpaF&ref_=pd_gw_ci_mcx_mr_hp_d", "", 0);  
    //fetcherService.giveAmazonProduct(testProduct);
    //console.log(fetcherService.getProduct(2));
    this.fetcherService = fetcherService;
    this.clipboardApi = clipboardApi;
  }
  
  public async loadTestProduct(id: number): Promise<void>{
    var res = this.fetcherService.getProduct(id)
    ;(await res).subscribe(response =>{
      var tempString: string = JSON.stringify(response);
      var pJson =  JSON.parse(tempString);
      //console.log(pJson);
      var p: Product = new Product(pJson.url, pJson.name, pJson.price, pJson.imgUrl, pJson.id);
      this.products = this.products.concat(p);
      console.log("Num Products: " + this.products.length);

      var cardElement = document.createElement("mat-card");
      cardElement.setAttribute("id", "product:" + p.id);
      cardElement.setAttribute("class", "mat-card mat-focus-indicator");

      var imgElement = document.createElement("img");
      imgElement.setAttribute("src", p.img_url);
      imgElement.setAttribute("class", "product-img");
      //imgElement.style.maxHeight = "10em"

      var nameElement = document.createElement("h3");
      nameElement.setAttribute("class", "product-name");
      //nameElement.setAttribute("onclick", "window.location.href='" + p.url+"'")
      nameElement.innerText = p.name;

      var priceElement = document.createElement("h3");
      priceElement.setAttribute("class", "product-price");
      priceElement.innerText = "$" + p.price;

      var linkButtonElement = document.createElement("button");
      linkButtonElement.setAttribute("mat-button", "");
      linkButtonElement.setAttribute("_ngcontent-hgy-c46","");
      //linkButtonElement.setAttribute("(click)", "copyLink(" + p.url + ")");
      linkButtonElement.addEventListener('click', (e) => {this.copyLink(p.url)} );
      linkButtonElement.setAttribute("class", "product-link mat-focus-indicator mat-button mat-button-base");
      linkButtonElement.innerText = "Copy Link"
 
      cardElement.appendChild(nameElement);
      cardElement.appendChild(imgElement);
      cardElement.appendChild(linkButtonElement);
      cardElement.appendChild(priceElement);

      document.getElementById("product-list-wrapper")?.appendChild(cardElement); //? because it could be null
      //document.getElementById("product:" + p.id)?.appendChild(imgElement); //? because it could be null
    });
  }

  copyLink(url: string): void{
    this.clipboardApi.copyFromContent(url);
  }

  ngOnInit(): void{
  }

}
