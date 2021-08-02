import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http'
import { Product } from './ts/product';

@Injectable({
  providedIn: 'root'
})
export class FetcherService {

  constructor(private httpClient: HttpClient) { }

  public get(url: string): Observable<any>{
    return this.httpClient.get(url);
  }

  public giveAmazonProduct(product: Product){
    // console.log("BODY:");
    // console.log(JSON.stringify(product));
    // console.log("END OF BODY");
    this.httpClient.post("https://localhost:44363/api/Product/AddAmazonProduct", JSON.parse(JSON.stringify(product))).subscribe(
      //(response) => console.log(response),
      //(error) => console.log(error),
    );
  }

  public async getProduct(id: number): Promise<Observable<any>> {
    //https://localhost:44363/api/Product/GetProduct?id=2
    return this.httpClient.get("https://localhost:44363/api/Product/GetProduct?id=" + id);
  }

  public async getAllProducts(): Promise<Observable<any>> {
    //https://localhost:44363/api/Product/GetProduct?id=2
    return this.httpClient.get("https://localhost:44363/api/Product/GetAllProducts");
  }

  public async deleteProduct(id: number): Promise<Observable<any>> {
    //https://localhost:44363/api/Product/GetProduct?id=2
    return this.httpClient.delete("https://localhost:44363/api/Product/RemoveProduct?id=" + id, {responseType: 'text'});
  }
}
