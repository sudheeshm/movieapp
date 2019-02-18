import { Component, OnInit, Inject } from '@angular/core';
import { Http } from '@angular/http';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Movie, MovieDetail, MoviePrice } from '../../../model/movie';

@Component({
    selector: 'movie-detail',
    templateUrl: './movie-detail.component.html',
    styleUrls: ['../app/app.component.css']
})

export class MovieDetailComponent implements OnInit
{
    private movieId: any;
    private movie: any;
    private movieprices: any;

    constructor(private route: ActivatedRoute, private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        console.log("movie-detail constructor() called");
    }

    ngOnInit() {
        console.log("movie-detail ngOnInit() called");
        //let id = this.route.snapshot.paramMap.get('id');
        this.route.paramMap.subscribe((params: ParamMap) => {
            let id = params.get('id');
            this.movieId = id;
        });

        this.http.get(this.baseUrl + 'api/movie?id=' + this.movieId.toString()).subscribe(result => {
            //this.movieprices = result.json() as MoviePrice;
            this.movie = result.json() as Movie;

            console.log(this.movie.detail.language);
            console.log(this.movie.detail.priceDetail[0].provider);
        }, error => console.error(error));
    }
}