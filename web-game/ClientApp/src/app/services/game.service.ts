import {Inject, Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";

import {catchError} from "rxjs/operators";
import {observable, Observable, throwError} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GameService {

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {

  }

  getRandomNumber(): Observable<{randomNumber:number}>{
    return this.http.get<{randomNumber:number}>(`${this.baseUrl}api/game`).pipe(catchError((error: any) => throwError(error)));
  }

  submit(number:number){
    return this.http.post(`${this.baseUrl}api/game`, {Number:number}).pipe(catchError((error: any) => throwError(error)));
  }
}
