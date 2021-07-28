import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http'

@Injectable({
  providedIn: 'root'
})
export class FetcherService {

  constructor(private httpClient: HttpClient) { }

  public get(url: string): Observable<any>{
    return this.httpClient.get(url);
  }
}
