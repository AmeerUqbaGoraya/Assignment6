import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'roleDisplay',
  standalone: true
})
export class RoleDisplayPipe implements PipeTransform {

  transform(value: string | undefined | null): string {
    if (!value) return '';
    
    switch (value) {
      case 'Admin':
        return 'Administrator';
      case 'Receptionist':
        return 'Receptionist';
      case 'Doctor':
        return 'Doctor';
      default:
        return value;
    }
  }

}
