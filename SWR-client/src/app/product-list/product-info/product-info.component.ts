import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Product } from 'src/app/ts/product';
import { ProductListComponent } from '../product-list.component';

@Component({
  selector: 'app-product-info',
  templateUrl: './product-info.component.html',
  styleUrls: ['./product-info.component.scss']
})
export class ProductInfoComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: {product: Product}) { 
    console.log("Info: " + data.product.name);
  }

  ngOnInit(): void {
  }

}
