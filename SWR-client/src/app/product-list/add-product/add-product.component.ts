import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormControl, Validators } from '@angular/forms';
import { ThemePalette } from '@angular/material/core';
import { ProgressBarMode } from '@angular/material/progress-bar';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html',
  styleUrls: ['./add-product.component.scss']
})
export class AddProductComponent implements OnInit {

  productURL = new FormControl('');

  @Output() addButtonClicked = new EventEmitter();

   //Settings for progres bar.
   color: ThemePalette = 'primary';
   value = 100;
   bufferValue = 100;
  
  constructor() { }

  buttonClicked(): void{
    //console.log("BUTTON CLICKED");
    //console.log(this.productURL.value);
    if(this.productURL.value != "")
      this.addButtonClicked.emit(this.productURL.value);
  }

  ngOnInit(): void {
  }

}
