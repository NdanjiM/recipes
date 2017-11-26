import { Component, OnInit } from '@angular/core';
import { Http } from '@angular/http'
@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {
    constructor(private _httpService: Http) { }
    apiValues: string[] = [];
    ngOnInit() {
        this._httpService.get('/api/recipes/recipes').subscribe(values => {
            this.apiValues = values.json() as string[];
        });
	};
	//login() {

	//	var user = this.password;
	//	var pass = $scope.username;
	//	var body = {username: user, password: pass };
	//	this._httpService.post('/api/recipes/recipes', body)).subscribe(values => {

	//	});
	//}
}