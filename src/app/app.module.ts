import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { HomeComponent } from './components/home/home.component';
import { CourseComponent } from './components/course/course.component';

const appRoutes: Routes = [
	//{ path: 'home', component: HomeComponent },
	{ path: 'course', component: CourseComponent },
	{
		path: '',
		//redirectTo: '/home',
		component: HomeComponent,
		pathMatch: 'full'
	}
];

@NgModule({
	declarations: [
		AppComponent,
		HomeComponent,
		CourseComponent
	],
	imports: [
		RouterModule.forRoot(appRoutes),
		BrowserModule
	],
	providers: [],
	bootstrap: [AppComponent]
})
export class AppModule { }
