import { Component, OnInit } from '@angular/core';
import { FetcherService } from '../fetcher.service';
import { Product } from '../ts/product';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {


  private fetcherService: FetcherService; 
  public products: Product[] = [];

  constructor(fetcherService: FetcherService){
    //fetcherService.getTestProdcut("https://www.amazon.com/Apple-AirPods-Charging-Latest-Model/dp/B07PXGQC1Q/ref=sr_1_1_sspa?dchild=1&keywords=airpods&qid=1627507012&sr=8-1-spons&psc=1&spLa=ZW5jcnlwdGVkUXVhbGlmaWVyPUExQ1BLQUwwTU9EU1VBJmVuY3J5cHRlZElkPUEwMzUyMTQxMVlDQkZPREZROElQMiZlbmNyeXB0ZWRBZElkPUEwMzA4NzczMkMyM1hTQVYyRjFMVyZ3aWRnZXROYW1lPXNwX2F0ZiZhY3Rpb249Y2xpY2tSZWRpcmVjdCZkb05vdExvZ0NsaWNrPXRydWU=").subscribe(response => {
    //var testProduct = new Product("https://www.amazon.com/JBL-Waterproof-Portable-Bluetooth-Speaker/dp/B07QK18BNY/?_encoding=UTF8&pd_rd_w=BD46g&pf_rd_p=620e7d0f-07bf-4434-8cbc-ed3abf0cf403&pf_rd_r=654DDX2X8RMC2QF9E003&pd_rd_r=aa8a8adb-73ef-47c9-ab0f-9c6569c95f3b&pd_rd_wg=BVpaF&ref_=pd_gw_ci_mcx_mr_hp_d", "", 0);  
    //fetcherService.giveAmazonProduct(testProduct);
    //console.log(fetcherService.getProduct(2));
    this.fetcherService = fetcherService;
  }

  public loadTestProduct(): void{
    
  }

  ngOnInit(): void{
  }

}
