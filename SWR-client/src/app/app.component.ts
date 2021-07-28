import { Component } from '@angular/core';
import { FetcherService } from './fetcher.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'SWR-client';

  constructor(fetcherService: FetcherService){
    fetcherService.get("https://localhost:44363/api/Fetcher").subscribe(response => {
      console.log("RESPONSE RECIEVED! :)");
      console.log(response);
    });
  }
}
