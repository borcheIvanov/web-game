import {Component, OnInit} from '@angular/core';
import {GameService} from "../services/game.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit{

  currentNumber: number;

  constructor(private gameService: GameService){}

  ngOnInit(): void {
    this.getRandomNumber();
  }

  getRandomNumber(){
    this.gameService.getRandomNumber().subscribe((result:{randomNumber:number})=>{
      if (result){
        this.currentNumber = result.randomNumber;
      }
    });
  }

  submitNumber(){
    this.gameService.submit(this.currentNumber).subscribe(result => console.log(result));
  }

}
