import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Product } from 'src/app/ts/product';
import { ProductListComponent } from '../product-list.component';

@Component({
  selector: 'app-product-info',
  templateUrl: './product-info.component.html',
  styleUrls: ['./product-info.component.scss']
})
export class ProductInfoComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<ProductInfoComponent>, @Inject(MAT_DIALOG_DATA) public data: {product: Product}) { 
    //console.log("Info: " + data.product.name);
    //this.dialogRef.close({ data: 'data' });
    
    
  }

  public deleteButtonClicked(){
    this.dialogRef.close({ product: this.data.product, delete: true })
  }

  public closeDialog(){
    this.dialogRef.close();
  }

  ngOnInit(): void {
    var name = document.createElement("li");
    name.innerText = this.data.product.name;
    document.getElementById("product-info-list")?.appendChild(name);

    var wip = document.createElement("li");
    wip.innerText = "TODO: More detailed info here.";
    document.getElementById("product-info-list")?.appendChild(wip);
    //console.log(document.getElementById("product-info-list"));
  }

}
